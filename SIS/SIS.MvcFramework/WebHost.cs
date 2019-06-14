using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.MvcFramework.Attributes.Action;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Attributes.Security;
using SIS.MvcFramework.Attributes.Validation;
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

        private static void ValidateBindingModels(Controller controller, IEnumerable<object> bindingModels)
        {
            foreach (object obj in bindingModels)
            {
                obj.GetType().GetProperties()
                    .ToList()
                    .ForEach(property =>
                    {
                        string propertyName = property.Name;

                        IEnumerable<ValidationSisAttribute> validationAttributes = property
                            .GetCustomAttributes<ValidationSisAttribute>();

                        validationAttributes
                            .Where(attr => !attr.IsValid(property.GetValue(obj)))
                            .ToList()
                            .ForEach(attr =>
                            {
                                controller.ModelState
                                    .AddErrorMessage(propertyName, attr.ErrorMessage);
                            });

                        validationAttributes.Where(attr => attr.GetType() == typeof(PasswordSisAttribute))
                            .Cast<PasswordSisAttribute>()
                            .ToList()
                            .ForEach(passwordAttribute =>
                            {
                                PropertyInfo confirmPasswordProperty = obj.GetType()
                                    .GetProperty(passwordAttribute.ConfirmPasswordPropertyName);

                                if (confirmPasswordProperty == null)
                                {
                                    throw new ArgumentException(PasswordSisAttribute.ConfirmPasswordPropertyNotFound);
                                }

                                if (property.GetValue(obj).ToString() != confirmPasswordProperty.GetValue(obj).ToString())
                                {
                                    controller.ModelState
                                        .AddErrorMessage(
                                            propertyName,
                                            PasswordSisAttribute.PasswordsDoNotMatchMessage);
                                }
                            });
                    });
            }
        }

        private static Controller CreateController(Type controllerType, IServiceProvider serviceProvider)
        {
            return (Controller) serviceProvider.CreateInstance(controllerType);
        }        

        private static IHttpResponse ProcessRequest(
            IHttpRequest request,
            IServiceProvider serviceProvider,
            MethodInfo action,
            Type controllerType)
        {
            Controller controller = CreateController(controllerType, serviceProvider);

            controller.Request = request;

            Principal controllerPrincipal = controller.User;

            AuthorizeAttribute controllerAuthAttr = controllerType.GetCustomAttribute<AuthorizeAttribute>();
            AuthorizeAttribute actionAuthAttr = action.GetCustomAttribute<AuthorizeAttribute>();

            if ((controllerAuthAttr != null && !controllerAuthAttr.IsAuthorized(controllerPrincipal))
            || (actionAuthAttr != null && !actionAuthAttr.IsAuthorized(controllerPrincipal)))
            {
                return new RedirectResult(UnauthorizedRedirectUrl);
            }

            ParameterInfo[] actionParameters = parametersByActionByControllerType[controllerType][action];

            object[] actionParameterObjects = ActionParametersMapper.GetActionObjectParameters(actionParameters, request);

            ValidateBindingModels(
                controller, 
                actionParameterObjects.Where(obj => obj.GetType().IsClass
                    && !typeof(IEnumerable).IsAssignableFrom(obj.GetType())));

            return (IHttpResponse) action.Invoke(controller, actionParameterObjects);
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
                    .Where((action => (action.GetCustomAttribute<NonActionAttribute>() == null
                        && typeof(IHttpResponse).IsAssignableFrom(action.ReturnType)
                        && action.DeclaringType == controllerType)))
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
                            req => ProcessRequest(
                                req, 
                                serviceProvider, 
                                action, 
                                controllerType));
                    });
            }
        }

        public static void Start<MvcApplication>()
            where MvcApplication : IMvcApplication, new()
        {
            IMvcApplication app = (IMvcApplication) Activator.CreateInstance(typeof(MvcApplication));

            IServiceProvider serviceProvider = new ServiceProvider();
            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            IHttpSessionStorage httpSessionStorage = new HttpSessionStorage();

            serviceProvider.Add<ILogger, ConsoleLogger>();

            app.ConfigureServices(serviceProvider);
            app.Configure();

            AutoRegisterRoutes(app, serverRoutingTable, serviceProvider);

            new Server(1300, serverRoutingTable, httpSessionStorage).Run();
        }
    }
}