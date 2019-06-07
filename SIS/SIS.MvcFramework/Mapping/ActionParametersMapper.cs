using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SIS.HTTP.Requests;

namespace SIS.MvcFramework.Mapping
{
    public class ActionParametersMapper
    {
        private static IEnumerable<PropertyInfo> GetBaseProperties(Type type)
            => type.GetProperties()
                .Where(pi => pi.PropertyType.IsPrimitive
                    || new[] { typeof(string), typeof(decimal) }.Contains(pi.PropertyType));

        private static ISet<string> TryGetHttpParameter(string parameterName, IHttpRequest request)
        {
            parameterName = parameterName.ToUpper();

            if (request.QueryData
                .Any(kvp => kvp.Key.ToUpper() == parameterName))
            {
                return request.QueryData
                    .First(kvp => kvp.Key.ToUpper() == parameterName)
                    .Value;
            }
            else if (request.FormData
                .Any(kvp => kvp.Key.ToUpper() == parameterName))
            {
                return request.FormData
                    .First(kvp => kvp.Key.ToUpper() == parameterName)
                    .Value;
            }

            return null;
        }

        private static object ProcessBindingModel(Type modelType, IHttpRequest request)
        {
            if (modelType.GetConstructors()
                .All(ctor => ctor.GetParameters().Length > 0))
            {
                return null;
            }

            object model = Convert.ChangeType(Activator.CreateInstance(modelType), modelType);

            IEnumerable<PropertyInfo> baseProperties = GetBaseProperties(modelType);

            foreach (PropertyInfo pi in baseProperties)
            {
                string value = TryGetHttpParameter(pi.Name, request)?.FirstOrDefault();

                try
                {
                    pi.SetValue(model, Convert.ChangeType(value, pi.PropertyType));
                }
                catch { }
            }

            return model;
        }

        public static object[] GetActionObjectParameters(ParameterInfo[] actionParameters, IHttpRequest request)
        {
            object[] actionParameterObjects = new object[actionParameters.Length];

            for (int i = 0; i < actionParameters.Length; i++)
            {
                ParameterInfo parameter = actionParameters[i];

                string parameterName = parameter.Name;
                Type parameterType = parameter.ParameterType;

                ISet<string> httpDataValue = TryGetHttpParameter(parameterName, request);

                string httpStringValue = httpDataValue?.FirstOrDefault();
                object parameterValue = null;
                
                if (parameterType.IsPrimitive
                    || new[] { typeof(string), typeof(decimal) }.Contains(parameterType))
                {
                    try
                    {
                        parameterValue = Convert.ChangeType(httpStringValue, parameter.ParameterType);
                    }
                    catch
                    {
                        parameterValue = Activator.CreateInstance(parameterType);
                    }
                }
                else if ((parameterType.IsArray || parameterType.IsGenericType
                    && typeof(IEnumerable).IsAssignableFrom(parameterType))
                    && httpDataValue != null)
                {
                    Type parameterGenericType = parameterType.IsArray
                        ? parameterType.GetElementType()
                        : parameterType.GenericTypeArguments[0];

                    try
                    {
                        Type listGenericType = typeof(List<>)
                            .MakeGenericType(parameterGenericType);
                        IList list = (IList) Activator.CreateInstance(listGenericType);
                        
                        foreach (object item in httpDataValue)
                        {
                            list.Add(Convert.ChangeType(item, parameterGenericType));
                        }

                        if (parameterType.IsArray)
                        {
                            Array array = Array.CreateInstance(parameterGenericType, list.Count);

                            list.CopyTo(array, 0);
                            
                            parameterValue = array;
                        }
                        else
                        {
                            if (parameterType.Name.Contains("Set"))
                            {
                                throw new NotImplementedException("Sets are not supported for action parameters");
                            }
                            else
                            {
                                // IEnumerable and list
                                parameterValue = list;
                            }
                        }
                    }
                    catch
                    { }
                }
                else
                {
                    parameterValue = ProcessBindingModel(parameterType, request);
                }

                actionParameterObjects[i] = parameterValue;
            }

            return actionParameterObjects;
        }
    }
}