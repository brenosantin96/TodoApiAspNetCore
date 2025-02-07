using ApiBreno01.Models;
using Microsoft.EntityFrameworkCore;
using Task = ApiBreno01.Models.Task;

namespace ApiBreno01.Context {
    public class ConnectionContext : DbContext {
        public ConnectionContext(DbContextOptions options) : base(options) {
                        
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Task> Tasks { get; set; } = null!;

    }
}
