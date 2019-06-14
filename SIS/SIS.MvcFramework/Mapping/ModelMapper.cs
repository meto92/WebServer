using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace SIS.MvcFramework.Mapping
{
    public static class ModelMapper
    {
        private static void MapProperty(object originInstance, object destinationInstance, PropertyInfo originProperty, PropertyInfo destinationProperty)
        {
            Type originPropertyType = originProperty.PropertyType;
            Type destinationPropertyType = destinationProperty.PropertyType;

            if (destinationPropertyType == typeof(string))
            {
                destinationProperty.SetValue(
                    destinationInstance,
                     originProperty.GetValue(originInstance).ToString());
            }
            else if (originPropertyType.IsPrimitive
                || new[] { typeof(string), typeof(decimal) }.Contains(originPropertyType)
                || originPropertyType == typeof(DateTime)
                || destinationPropertyType == typeof(DateTime))
            {
                if (originPropertyType != destinationPropertyType)
                {
                    try
                    {
                        destinationProperty.SetValue(
                            destinationInstance,
                            Convert.ChangeType(
                                originProperty.GetValue(originInstance),
                                destinationPropertyType));
                    }
                    catch
                    { }
                }
                else
                {
                    destinationProperty.SetValue(
                        destinationInstance,
                        originProperty.GetValue(originInstance));
                }
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
                object originValue = originProperty.GetValue(originInstance);
                object destinationValue = MapObject(originValue, destinationPropertyType);

                destinationProperty.SetValue(destinationInstance, destinationValue);
            }
        }

        private static object MapObject(object origin, Type destinationType)
        {
            object destinationInstance = Activator.CreateInstance(destinationType);

            foreach (PropertyInfo originProperty in origin.GetType().GetProperties())
            {
                PropertyInfo destinationProperty = destinationInstance.GetType()
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