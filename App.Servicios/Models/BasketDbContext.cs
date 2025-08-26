using Microsoft.EntityFrameworkCore;

namespace App.Servicios.Models
{
    public class BasketDbContext : DbContext
    {
        public BasketDbContext(DbContextOptions<BasketDbContext> options) 
            : base(options) 
        {
        
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Juego>().ToTable("juego");
        }

        public DbSet<Juego> Juego { get; set; } = null!;
    }
}
