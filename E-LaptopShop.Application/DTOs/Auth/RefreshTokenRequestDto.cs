using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "AccessToken token is required")]

        public string AccessToken { get; set; } = string.Empty;
    }
}
