using System;
using System.Collections;
using System.Reflection;

namespace SIS.MvcFramework.Mapping
{
    public static class ModelMapper
    {
        private static void MapProperty(object originInstance, object destinationInstance, PropertyInfo originProperty, PropertyInfo destinationProperty)
        {
            if (destinationProperty.PropertyType == typeof(string))
            {
                destinationProperty.SetValue(destinationInstance,
                    originProperty.GetValue(originInstance).ToString());
            }
            else if (typeof(IEnumerable).IsAssignableFrom(destinationProperty.PropertyType))
            {
                var originCollection = (IEnumerable) originProperty.GetValue(originInstance);
                var destinationElementType = destinationProperty.GetValue(destinationInstance)
                    .GetType()
                    .GetGenericArguments()[0];

                var destinationCollection = (IList) Activator.CreateInstance(destinationProperty.PropertyType);

                foreach (var originElement in originCollection)
                {
                    destinationCollection.Add(MapObject(originElement, destinationElementType));
                }

                destinationProperty.SetValue(destinationInstance, destinationCollection);
            }
            else
            {
                destinationProperty.SetValue(destinationInstance,
                    originProperty.GetValue(originInstance));
            }
        }

        private static object MapObject(object origin, Type destinationType)
        {
            object destinationInstance = Activator.CreateInstance(destinationType);
            
            foreach (PropertyInfo originProperty in origin.GetType().GetProperties())
            {
                PropertyInfo destinationProperty = destinationType.GetType()
                    .GetProperty(originProperty.Name);

                if (destinationProperty != null)
                {
                    MapProperty(origin, destinationInstance, originProperty, destinationProperty);
                }
            }

            return destinationInstance;
        }

        public static TDestination ProjectTo<TDestination>(object origin)
            => (TDestination) MapObject(origin, typeof(TDestination));
    }
}