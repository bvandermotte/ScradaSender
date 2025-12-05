using Microsoft.EntityFrameworkCore;
using ScradaSender.DataAccess.Entities;

namespace ScradaSender.DataAccess
{
    public class ScradaSenderDbContext(DbContextOptions<ScradaSenderDbContext> options) : DbContext(options)
    {
        public DbSet<FileStatusses> FileStatusses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}