namespace EduHuan.backend.DTOs
{
    public class ProductoImageDTO
    {
        public IFormFile? ImageFile { get; set; } // Para archivos locales
        public string? ImageUrl { get; set; } // Para URLs
    }
}
