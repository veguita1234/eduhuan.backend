using System.ComponentModel.DataAnnotations;

namespace EduHuan.backend.Repositories.Entities
{
    public class Producto
    {
        [Key]
        public Guid ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public decimal Precio { get; set; }
        public string Descripcion {  get; set; }
        public string? Imagen {  get; set; }
        public string Categoria { get; set; }

        public ICollection<Calificaciones> Calificaciones { get; set; }
        public ICollection<ConjuntoCalificaciones> ConjuntoCalificaciones { get; set; }


    }
}
