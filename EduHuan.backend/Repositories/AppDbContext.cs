using EduHuan.backend.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduHuan.backend.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Calificaciones> Calificaciones { get; set; }
        public DbSet<ConjuntoCalificaciones> ConjuntoCalificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calificaciones>()
                 .HasOne(c => c.Producto)
                 .WithMany(p => p.Calificaciones)
                 .HasForeignKey(c => c.ProductoId);

            modelBuilder.Entity<Calificaciones>()
                .HasOne(c => c.User)
                .WithMany(u => u.Calificaciones)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<ConjuntoCalificaciones>()
            .HasKey(cc => new { cc.ProductoId, cc.CalificacionId }); // Composite key if needed

            modelBuilder.Entity<ConjuntoCalificaciones>()
                .HasOne(cc => cc.Producto)
                .WithMany(p => p.ConjuntoCalificaciones)
                .HasForeignKey(cc => cc.ProductoId);

            modelBuilder.Entity<ConjuntoCalificaciones>()
                .HasOne(cc => cc.Calificaciones)
                .WithMany(c => c.ConjuntoCalificaciones)
                .HasForeignKey(cc => cc.CalificacionId);
        }
    }
}
