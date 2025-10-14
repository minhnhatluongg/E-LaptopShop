using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Common
{
    public class SlugGenerator : ISlugGenerator
    {
        private static readonly Regex NonWord = new(@"[^a-z0-9\- ]", RegexOptions.Compiled);
        private static readonly Regex MultiDash = new(@"-+", RegexOptions.Compiled);

        private readonly ApplicationDbContext _db;
        public SlugGenerator(ApplicationDbContext db) => _db = db;
        public async Task<string> GenerateSlugAsync(string name, string? entitySet, int? excludeId = null, CancellationToken ct = default)
        {
            var baseSlug = ToSlugCore(name);

            if (string.IsNullOrWhiteSpace(entitySet))
                return baseSlug;

            var candidate = baseSlug;
            var i = 1;

            while (await ExistsAsync(entitySet, candidate, excludeId, ct))
            {
                i++;
                candidate = $"{baseSlug}-{i}";
                if (candidate.Length > 200) candidate = candidate[..200];
            }
            return candidate;
        }

        private static string ToSlugCore(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "n-a";
            s = RemoveDiacritics(s).ToLowerInvariant();
            s = NonWord.Replace(s, "");
            s = s.Replace(' ', '-');
            s = MultiDash.Replace(s, "-").Trim('-');
            return string.IsNullOrEmpty(s) ? "n-a" : s;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);
            foreach (var ch in normalized)
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) sb.Append(ch);
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private Task<bool> ExistsAsync(string entitySet, string slug, int? excludeId, CancellationToken ct)
            => entitySet switch
            {
                "Product" => _db.Products.AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId), ct),
                "Category" => _db.Categories.AnyAsync(c => c.Slug == slug && (excludeId == null || c.Id != excludeId), ct),
                _ => Task.FromResult(false)
            };
    }
}
