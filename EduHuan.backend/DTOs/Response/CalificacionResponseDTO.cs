namespace EduHuan.backend.DTOs.Response
{
    public class CalificacionResponseDTO
    {
        public Guid CalificacionId { get; set; }
        public decimal Calificacion { get; set; }
        public string CalificacionTexto { get; set; }
        public DateTime FechaCalificacion { get; set; }
        public Guid ProductoId { get;  set; }
        public Guid UserId { get;  set; }
    }

}
