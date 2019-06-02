using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SIS.MvcFramework.Routing;
using SIS.HTTP.Responses;
using SIS.MvcFramework.Attributes.Methods;
using SIS.HTTP.Enums;

namespace SIS.MvcFramework
{
    public static class WebHost
    {
        private static readonly IDictionary<Type, object> controllerInstances;
        private static readonly IDictionary<Type, Dictionary<MethodInfo, ParameterInfo[]>> parametersByActionByControllerType;

        static WebHost()
        {
            controllerInstances = new Dictionary<Type, object>();
            parametersByActionByControllerType = new Dictionary<Type, Dictionary<MethodInfo, ParameterInfo[]>>();
        }

        private static Controller CreateController(Type controllerType)
        {
            return (Controller) Activator.CreateInstance(controllerType);
        }

        private static void AutoRegisterRoutes(IMvcApplication app, IServerRoutingTable serverRoutingTable)
        {
            Type[] controllerTypes = app.GetType().Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                    && t.Name.EndsWith(nameof(Controller))
                    && typeof(Controller).IsAssignableFrom(t))
                .ToArray();

            foreach (Type controllerType in controllerTypes)
            {
                controllerInstances[controllerType] = CreateController(controllerType);
                parametersByActionByControllerType[controllerType] = new Dictionary<MethodInfo, ParameterInfo[]>();

                controllerType.GetMethods()
                    .Where(action => typeof(IHttpResponse).IsAssignableFrom(action.ReturnType)
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
                                Controller controller = (Controller) controllerInstances[controllerType];

                                controller.Request = req;
                                object[] actionParameterObjects = ActionParameterMapper.Instance.MapActionParameters(parametersByActionByControllerType[controllerType][action], req);

                                return (IHttpResponse) action.Invoke(controller, actionParameterObjects);
                            });
                    });
            }
        }

        public static void Start(IMvcApplication app)
        {
            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            app.ConfigureServices();
            app.Configure();

            AutoRegisterRoutes(app, serverRoutingTable);

            new Server(80, serverRoutingTable).Run();
        }
    }
}