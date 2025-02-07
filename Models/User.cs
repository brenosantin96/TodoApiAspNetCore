using System.ComponentModel.DataAnnotations;

namespace ApiBreno01.Models
{
    public class User
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(4, ErrorMessage = "A senha deve ter pelo menos 4 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
