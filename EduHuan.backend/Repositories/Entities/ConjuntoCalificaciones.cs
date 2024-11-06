using System.ComponentModel.DataAnnotations;

namespace EduHuan.backend.Repositories.Entities
{
    public class ConjuntoCalificaciones
    {
        [Key]
        public Guid ProductoId { get; set; }
        public Guid CalificacionId { get; set; }
        public Producto Producto { get; set; }
        public Calificaciones Calificaciones { get; set; }
    }
}
