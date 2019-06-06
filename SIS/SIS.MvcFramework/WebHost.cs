using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SIS.HTTP.Enums;
using SIS.HTTP.Responses;
using SIS.MvcFramework.Attributes.Action;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Attributes.Security;
using SIS.MvcFramework.DependencyContainer;
using SIS.MvcFramework.Identity;
using SIS.MvcFramework.Logging;
using SIS.MvcFramework.Mapping;
using SIS.MvcFramework.Results;
using SIS.MvcFramework.Routing;
using SIS.MvcFramework.Sessions;

using IServiceProvider = SIS.MvcFramework.DependencyContainer.IServiceProvider;

namespace SIS.MvcFramework
{
    public static class WebHost
    {
        private const string UnauthorizedRedirectUrl = "/Users/Login";

        private static readonly IDictionary<Type, Dictionary<MethodInfo, ParameterInfo[]>> parametersByActionByControllerType;

        static WebHost()
        {
            parametersByActionByControllerType = new Dictionary<Type, Dictionary<MethodInfo, ParameterInfo[]>>();
        }

        private static Controller CreateController(Type controllerType, IServiceProvider serviceProvider)
        {
            return (Controller) serviceProvider.CreateInstance(controllerType);
        }

        private static void AutoRegisterRoutes(IMvcApplication app, IServerRoutingTable serverRoutingTable, IServiceProvider serviceProvider)
        {
            Type[] controllerTypes = app.GetType().Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                    && t.Name.EndsWith(nameof(Controller))
                    && typeof(Controller).IsAssignableFrom(t))
                .ToArray();

            foreach (Type controllerType in controllerTypes)
            {
                parametersByActionByControllerType[controllerType] = new Dictionary<MethodInfo, ParameterInfo[]>();

                controllerType.GetMethods()
                    .Where(action => action.GetCustomAttribute<NonActionAttribute>() == null
                        && typeof(IHttpResponse).IsAssignableFrom(action.ReturnType)
                        && action.DeclaringType == controllerType)
                    .ToList()
                    .ForEach(action =>
                    {
                        parametersByActionByControllerType[controllerType][action] = action.GetParameters();

                        HttpMethodAttribute httpMethodAttribute = action.GetCustomAttribute<HttpMethodAttribute>();
                        
                        string actionName = httpMethodAttribute?.ActionName ?? action.Name;
                        HttpRequestMethod httpRequestMethod = httpMethodAttribute?.HttpMethod ?? HttpRequestMethod.Get;
                        string path = httpMethodAttribute?.Path ?? $"/{controllerType.Name.Replace(nameof(Controller), string.Empty)}/{actionName}";
                        
                        serverRoutingTable.Add(
                            httpRequestMethod,
                            path,
                            req =>
                            {
                                Controller controller = CreateController(controllerType, serviceProvider);

                                controller.Request = req;

                                Principal controllerPrincipal = controller.User;

                                AuthorizeAttribute controllerAuthAttr = controllerType.GetCustomAttribute<AuthorizeAttribute>();
                                AuthorizeAttribute actionAuthAttr = action.GetCustomAttribute<AuthorizeAttribute>();

                                if ((controllerAuthAttr != null && !controllerAuthAttr.IsAuthorized(controllerPrincipal))
                                || (actionAuthAttr != null && !actionAuthAttr.IsAuthorized(controllerPrincipal)))
                                {
                                    return new RedirectResult(UnauthorizedRedirectUrl);
                                }
                                
                                object[] actionParameterObjects = ActionParameterMapper.Instance.MapActionParameters(parametersByActionByControllerType[controllerType][action], req);

                                return (IHttpResponse) action.Invoke(controller, actionParameterObjects);
                            });
                    });
            }
        }

        public static void Start(IMvcApplication app)
        {
            IServiceProvider serviceProvider = new ServiceProvider();
            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            IHttpSessionStorage httpSessionStorage = new HttpSessionStorage();

            serviceProvider.Add<ILogger, ConsoleLogger>();

            app.ConfigureServices(serviceProvider);
            app.Configure();

            AutoRegisterRoutes(app, serverRoutingTable, serviceProvider);

            new Server(80, serverRoutingTable, httpSessionStorage).Run();
        }
    }
}