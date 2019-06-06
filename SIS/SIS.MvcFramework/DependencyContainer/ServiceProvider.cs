using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SIS.MvcFramework.DependencyContainer
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly IDictionary<Type, Type> dependencyContainer;

        public ServiceProvider()
            => this.dependencyContainer = new ConcurrentDictionary<Type, Type>();

        public void Add<TSource, TDestination>()
            where TDestination : TSource
            => this.dependencyContainer[typeof(TSource)] = typeof(TDestination);

        public object CreateInstance(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(ctor => ctor.GetParameters().Length)
                .FirstOrDefault();

            if (constructor == null)
            {
                return null;
            }

            object[] parameters = constructor.GetParameters()
                .Select(param => this.CreateInstance(this.dependencyContainer
                    .ContainsKey(param.ParameterType) 
                        ? this.dependencyContainer[param.ParameterType] 
                        : param.ParameterType))
                .ToArray();

            return constructor.Invoke(parameters);
        }

        public T CreateInstance<T>()
            => (T) this.CreateInstance(typeof(T));
    }
}