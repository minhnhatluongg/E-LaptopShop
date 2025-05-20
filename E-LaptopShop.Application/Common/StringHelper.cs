using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common
{
    public static class StringHelper
    {
        public static string Slugify(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Chuyển về chữ thường và loại bỏ dấu
            text = text.ToLowerInvariant();
            text = RemoveDiacritics(text);

            // Thay thế các ký tự đặc biệt bằng dấu gạch ngang
            text = Regex.Replace(text, @"[^a-z0-9\s-.]", "");

            // Thay thế khoảng trắng bằng dấu gạch ngang
            text = Regex.Replace(text, @"\s+", "-");

            // Loại bỏ nhiều dấu gạch ngang liên tiếp
            text = Regex.Replace(text, @"-+", "-");

            // Cắt bớt nếu quá dài
            if (text.Length > 100)
                text = text.Substring(0, 100);

            return text;
        }

        public static string RemoveDiacritics(string text)
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

        public static string SlugifyFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;

            string extension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            // Slugify chỉ phần tên file, giữ nguyên phần mở rộng
            string slugifiedName = Slugify(fileNameWithoutExtension);

            return slugifiedName + extension;
        }
    }
}
