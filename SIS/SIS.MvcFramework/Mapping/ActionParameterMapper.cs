using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SIS.HTTP.Requests;

namespace SIS.MvcFramework.Mapping
{
    public class ActionParameterMapper
    {
        private static ActionParameterMapper instance;

        private ActionParameterMapper() { }

        public static ActionParameterMapper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ActionParameterMapper();
                }

                return instance;
            }
        }

        private bool TryGetParameterFromRequest(string paramName, IHttpRequest request, out object obj)
            => request.QueryData
                .TryGetValue(paramName, out obj)
                    || request.FormData
                        .TryGetValue(paramName, out obj);

        private IEnumerable<PropertyInfo> GetPrimitiveAndStringProperties(Type type)
            => type.GetProperties()
                .Where(pi => pi.PropertyType.IsPrimitive
                    || pi.PropertyType == typeof(string));

        private bool HasDefaultConstructor(Type type)
           => type.GetConstructors()
               .Any(ci => ci.GetParameters().Length == 0);

        private object ProcessPrimitiveParameter(string parameterName, Type parameterType, IHttpRequest request)
        {
            bool hasValueInRequest = this.TryGetParameterFromRequest(parameterName, request, out object obj);

            try
            {
                return Convert.ChangeType(obj, parameterType);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private object ProcessBindingModel(Type modelType, IHttpRequest request)
        {
            object model = Convert.ChangeType(Activator.CreateInstance(modelType), modelType);

            IEnumerable<PropertyInfo> modelPrimitiveAndStringProperties = this.GetPrimitiveAndStringProperties(modelType);

            foreach (PropertyInfo pi in modelPrimitiveAndStringProperties)
            {
                bool hasValueInRequest = this.TryGetParameterFromRequest(pi.Name, request, out object value);

                try
                {
                    pi.SetValue(model, Convert.ChangeType(value, pi.PropertyType));
                }
                catch (FormatException) { }
            }

            return model;
        }

        public object[] MapActionParameters(ParameterInfo[] actionParameters, IHttpRequest req)
        {
            object[] mappedParameters = new object[actionParameters.Length];

            for (int i = 0; i < actionParameters.Length; i++)
            {
                ParameterInfo parameter = actionParameters[i];

                string parameterName = parameter.Name;
                Type parameterType = parameter.ParameterType;

                if (parameterType.IsPrimitive || parameterType == typeof(string))
                {
                    mappedParameters[i] = ProcessPrimitiveParameter(parameterName, parameterType, req);
                }
                else if (this.HasDefaultConstructor(parameterType))
                {
                    object bindingModel = this.ProcessBindingModel(parameterType, req);

                    mappedParameters[i] = bindingModel;
                }
            }

            return mappedParameters;
        }
    }
}