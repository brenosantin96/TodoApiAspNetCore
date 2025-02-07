using System.ComponentModel.DataAnnotations;

namespace ApiBreno01.Models
{
    public class Task
    {
        
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome da tarefa é obrigatório.")]
        [MinLength(2, ErrorMessage = "A senha deve ter pelo menos 2 caracteres.")]
        public string Name { get; set; }
        public bool IsFinished { get; set; }
    }
}
