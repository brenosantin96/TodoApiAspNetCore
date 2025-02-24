using System.ComponentModel.DataAnnotations;

namespace ApiBreno01.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Mail is required")]
        [EmailAddress(ErrorMessage = "This mail is invalid.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(4, ErrorMessage = "Password must have at least 4 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
