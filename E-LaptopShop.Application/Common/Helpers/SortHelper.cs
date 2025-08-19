using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using System.Linq.Expressions;
using System.Reflection;

namespace E_LaptopShop.Application.Common.Helpers
{
    /// <summary>
    /// Generic sorting helper
    /// </summary>
    public static class SortHelper
    {
        /// <summary>
        /// Apply dynamic sorting based on property name
        /// </summary>
        public static IEnumerable<T> ApplyDynamicSorting<T>(
            IEnumerable<T> entities, 
            SortingOptions sort) where T : class
        {
            if (!sort.HasSorting)
                return entities;

            var propertyInfo = typeof(T).GetProperty(sort.SortBy!, 
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null)
                return entities;

            return sort.IsAscending
                ? entities.OrderBy(e => propertyInfo.GetValue(e))
                : entities.OrderByDescending(e => propertyInfo.GetValue(e));
        }

        /// <summary>
        /// Apply sorting with custom mapping (for calculated fields, navigation properties)
        /// </summary>
        public static IEnumerable<T> ApplyCustomSorting<T>(
            IEnumerable<T> entities,
            SortingOptions sort,
            Dictionary<string, Func<T, object>> sortMappings) where T : class
        {
            if (!sort.HasSorting || !sortMappings.ContainsKey(sort.SortBy!.ToLower()))
                return entities;

            var sortFunc = sortMappings[sort.SortBy!.ToLower()];

            return sort.IsAscending
                ? entities.OrderBy(sortFunc)
                : entities.OrderByDescending(sortFunc);
        }
    }
}

