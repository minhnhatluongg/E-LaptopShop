namespace E_LaptopShop.Application.Models
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        
        // 2025 Best Practice: Short-lived access tokens
        public int AccessTokenExpirationMinutes { get; set; } = 15; // 15 minutes
        
        // 2025 Best Practice: Long-lived refresh tokens
        public int RefreshTokenExpirationDays { get; set; } = 7; // 7 days
        
        // Validation settings
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateAudience { get; set; } = true;
        public bool ValidateLifetime { get; set; } = true;
        public bool ValidateIssuerSigningKey { get; set; } = true;
        public int ClockSkewMinutes { get; set; } = 5;
        
        // 2025 Security: Enhanced settings
        public bool RequireHttpsMetadata { get; set; } = true;
        public bool SaveToken { get; set; } = false;
        public int MaxFailedAccessAttempts { get; set; } = 5;
        public int LockoutDurationMinutes { get; set; } = 30;
    }
}