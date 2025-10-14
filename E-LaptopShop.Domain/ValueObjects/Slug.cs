using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.ValueObjects
{
    public sealed class Slug : IEquatable<Slug>
    {
        private static readonly Regex Valid = new(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);
        public string Value { get; }

        public Slug(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Slug cannot be null or empty.", nameof(value));
            if (value.Length > 200)
                throw new ArgumentException("Slug too long.", nameof(value));
            if (!Valid.IsMatch(value)) throw new ArgumentException("Slug format invalid", nameof(value));
            Value = value;
        }
        public override string ToString() => Value;
        public bool Equals(Slug? other)
        {
            return other is not null && Value == other.Value;
        }
        public override bool Equals(object? obj) => obj is Slug s && Equals(s);
        public override int GetHashCode() => Value.GetHashCode();
        public static implicit operator string(Slug s) => s.Value;
    }
}
