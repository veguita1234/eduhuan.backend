using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EduHuan.backend.Repositories.Entities
{
    public class Calificaciones
    {
        [Key]
        public Guid CalificacionId { get; set; }
        public Guid ProductoId { get; set; }
        public Guid UserId { get; set; }
        public decimal Calificacion {  get; set; }
        public DateTime FechaCalificacion { get; set; }
        public string CalificacionTexto { get; set; }

        public  Producto Producto { get; set; }
        public  User User { get; set; }

        public ICollection<ConjuntoCalificaciones> ConjuntoCalificaciones { get; set; }

    }
}
