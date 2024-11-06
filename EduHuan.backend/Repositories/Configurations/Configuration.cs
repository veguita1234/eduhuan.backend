using EduHuan.backend.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduHuan.backend.Repositories.Configurations
{
    public class Configuration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(x => x.UserId);
            builder.Property(x => x.UserId).HasColumnName("UserId");
            builder.Property(x => x.Tipo).HasColumnName("Tipo");
            builder.Property(x => x.Name).HasColumnName("Name");
            builder.Property(x => x.LastName).HasColumnName("LastName");
            builder.Property(x => x.Celular).HasColumnName("Celular");
            builder.Property(x => x.UserName).HasColumnName("UserName");
            builder.Property(x => x.Password).HasColumnName("Password");
        }

        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("Producto");
            builder.HasKey(x => x.ProductoId);
            builder.Property(x => x.ProductoId).HasColumnName("ProductoId");
            builder.Property(x => x.NombreProducto).HasColumnName("NombreProducto");
            builder.Property(x => x.Precio).HasColumnName("Precio");
            builder.Property(x => x.Descripcion).HasColumnName("Descripcion");
            builder.Property(x => x.Imagen).HasColumnName("Imagen");
            builder.Property(x => x.Categoria).HasColumnName("Categoria");
        }

        public void Configure(EntityTypeBuilder<Calificaciones> builder)
        {
            builder.ToTable("Calificaciones");
            builder.HasKey(x => x.CalificacionId);
            builder.Property(x => x.CalificacionId).HasColumnName("CalificacionId");
            builder.Property(x => x.ProductoId).HasColumnName("ProductoId");
            builder.Property(x => x.UserId).HasColumnName("UserId");
            builder.Property(x => x.Calificacion).HasColumnName("Calificacion");
            builder.Property(x => x.FechaCalificacion).HasColumnName("FechaCalificacion");
            builder.Property(x => x.CalificacionTexto).HasColumnName("CalificacionTexto");

        }

        public void Configure(EntityTypeBuilder<ConjuntoCalificaciones> builder)
        {
            builder.ToTable("ConjuntoCalificaciones");
            builder.Property(x => x.ProductoId).HasColumnName("ProductoId");
            builder.Property(x => x.CalificacionId).HasColumnName("CalificacionId");


        }
    }
}