using EduHuan.backend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHuan.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConjuntoCalificacionesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ConjuntoCalificacionesController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("byproduct/{productoId}")]
        public IActionResult GetCalificacionesByProducto(Guid productoId)
        {
            var calificaciones = _context.ConjuntoCalificaciones
                .Where(cc => cc.ProductoId == productoId)
                .Include(cc => cc.Calificaciones) // Include the entire Calificaciones entity
                .Include(cc => cc.Producto) // Include the related Producto as well
                .ToList();

            return Ok(calificaciones);
        }



    }
}
