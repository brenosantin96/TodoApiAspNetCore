﻿using System.ComponentModel.DataAnnotations;

namespace ApiBreno01.Models
{
    public class User
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Mail is required")]
        [EmailAddress(ErrorMessage = "This mail is invalid.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(4, ErrorMessage = "Password must have at least 4 characters.")]
        public string Password { get; set; } = string.Empty;

        // Propriedade de navegação para as tarefas do usuário
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
