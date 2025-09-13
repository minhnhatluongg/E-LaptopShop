using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace E_LaptopShop.Application.Common.Helpers
{
    public static class SearchHelper
    {
        /// <summary>
        /// Intelligent search with multiple strategies
        /// </summary>
        public static IEnumerable<T> ApplyIntelligentSearch<T>(
            IEnumerable<T> entities,
            SearchOptions search,
            Func<T, string?[]> getSearchableValues,
            Func<T, decimal?> getPriceValue = null)
        {
            if (!search.HasSearch) return entities;

            var searchTerm = NormalizeSearchTerm(search.SearchTerm!);

            return entities.Where(entity =>
            {
                // 1. Text-based search
                var searchableValues = getSearchableValues(entity);
                var hasTextMatch = searchableValues.Any(value =>
                    !string.IsNullOrEmpty(value) &&
                    NormalizeSearchTerm(value).Contains(searchTerm));

                // 2. Price-based search (if numeric and price available)
                var hasPriceMatch = false;
                if (decimal.TryParse(search.SearchTerm, out var searchPrice) && getPriceValue != null)
                {
                    var entityPrice = getPriceValue(entity);
                    hasPriceMatch = entityPrice.HasValue &&
                        IsInPriceRange(entityPrice.Value, searchPrice);
                }

                return hasTextMatch || hasPriceMatch;
            });
        }

        /// <summary>
        /// Legacy support for existing code
        /// </summary>
        public static IEnumerable<T> ApplyGenericSearch<T>(
            IEnumerable<T> entities,
            SearchOptions search,
            string[] searchFields)
        {
            if (!search.HasSearch) return entities;

            var searchTerm = NormalizeSearchTerm(search.SearchTerm!);

            return entities.Where(entity =>
            {
                return searchFields.Any(fieldName =>
                {
                    var property = typeof(T).GetProperty(fieldName);
                    if (property == null) return false;

                    var value = property.GetValue(entity)?.ToString();
                    return !string.IsNullOrEmpty(value) &&
                           NormalizeSearchTerm(value).Contains(searchTerm);
                });
            });
        }

        private static string NormalizeSearchTerm(string term)
        {
            if (string.IsNullOrEmpty(term)) return string.Empty;
            return RemoveDiacritics(term.ToLower().Trim());
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private static bool IsInPriceRange(decimal actualPrice, decimal searchPrice)
        {
            // ±20% price tolerance
            var tolerance = 0.2m;
            var lowerBound = searchPrice * (1 - tolerance);
            var upperBound = searchPrice * (1 + tolerance);

            return actualPrice >= lowerBound && actualPrice <= upperBound;
        }
    }
}