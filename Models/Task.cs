using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiBreno01.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome da tarefa é obrigatório.")]
        [MinLength(2, ErrorMessage = "A senha deve ter pelo menos 2 caracteres.")]
        public string Name { get; set; }

        public bool IsFinished { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateToFinish { get; set; } // Nullable DateTime
        public DateTime? DateFinished { get; set; } // Nullable DateTime

        //foreign key User
        public int UserId { get; set; }

        // Propriedade de navegação para o User
        [ForeignKey("UserId")]
        public User? User { get; set; }


    }
}

