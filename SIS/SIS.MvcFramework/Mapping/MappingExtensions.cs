using System.Collections.Generic;
using System.Linq;

namespace SIS.MvcFramework.Mapping
{
    public static class MappingExtensions
    {
        public static IEnumerable<TDestination> To<TDestination>(this IEnumerable<object> collection)
            => collection.Select(ModelMapper.ProjectTo<TDestination>);
    }
}