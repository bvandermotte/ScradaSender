using Microsoft.EntityFrameworkCore;

namespace ScradaSender.DataAccess
{
    public class ScradaSenderDbContext(DbContextOptions<ScradaSenderDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}