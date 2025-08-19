using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using System.Reflection;

namespace E_LaptopShop.Application.Common.Helpers
{
    /// <summary>
    /// Generic search helper for any entity type
    /// </summary>
    public static class SearchHelper
    {
        /// <summary>
        /// Apply generic search across multiple properties
        /// </summary>
        public static IEnumerable<T> ApplyGenericSearch<T>(
            IEnumerable<T> entities, 
            SearchOptions search, 
            string[]? defaultSearchFields = null) where T : class
        {
            if (!search.HasSearch)
                return entities;

            var searchTerm = search.SearchTerm!.ToLower();
            var searchFields = search.SearchFields ?? defaultSearchFields ?? GetDefaultSearchFields<T>();

            return entities.Where(entity =>
            {
                foreach (var field in searchFields)
                {
                    var value = GetPropertyValue(entity, field);
                    if (value != null && value.ToLower().Contains(searchTerm))
                        return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Get default searchable fields for an entity (string properties)
        /// </summary>
        public static string[] GetDefaultSearchFields<T>() where T : class
        {
            return typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanRead)
                .Select(p => p.Name)
                .ToArray();
        }

        /// <summary>
        /// Get property value safely with navigation property support
        /// </summary>
        public static string? GetPropertyValue<T>(T entity, string propertyPath) where T : class
        {
            try
            {
                object? value = entity;
                var properties = propertyPath.Split('.');

                foreach (var prop in properties)
                {
                    if (value == null) return null;

                    var propertyInfo = value.GetType().GetProperty(prop);
                    if (propertyInfo == null) return null;

                    value = propertyInfo.GetValue(value);
                }

                return value?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}

