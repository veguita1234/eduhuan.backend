using EduHuan.backend.Repositories.Entities;

namespace EduHuan.backend.DTOs
{
    public class CalificacionDTO
    {
        public Guid CalificacionId { get; set; }
        public Guid ProductoId { get; set; }
        public Guid UserId { get; set; }
        public decimal Calificacion { get; set; }
        public DateTime FechaCalificacion { get; set; }
        public string CalificacionTexto { get; set; }

    }
}
