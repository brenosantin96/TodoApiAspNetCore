using ApiBreno01.Models;
using Microsoft.EntityFrameworkCore;
using Task = ApiBreno01.Models.Task;

namespace ApiBreno01.Context {
    public class ConnectionContext : DbContext {
        public ConnectionContext(DbContextOptions options) : base(options) {
                        
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Task> Tasks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Task>()
                .HasOne(t => t.User) //Define que uma tarefa pertence a um usuário.
                .WithMany(t => t.Tasks) // Define que um usuário pode ter muitas tarefas
                .HasForeignKey(t => t.UserId); // Especifica a chave estrangeira (UserId) na tabela Task
        }

    }
}
