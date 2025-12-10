using System.ComponentModel.DataAnnotations;

namespace EcommerceWebAPI.Models.DTOs
{
    public class Login
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
