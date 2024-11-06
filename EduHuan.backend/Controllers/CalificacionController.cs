using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHuan.backend.DTOs;
using EduHuan.backend.DTOs.Response;
using EduHuan.backend.Repositories;
using EduHuan.backend.Repositories.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHuan.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalificacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public CalificacionController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("addcalificacion")]
        public async Task<IActionResult> AddCalificacionSimple([FromBody] CalificacionDTO calificacionDTO)
        {
            if (calificacionDTO == null)
            {
                return BadRequest("La calificación no puede ser nula.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productoExists = await _context.Producto.AnyAsync(p => p.ProductoId == calificacionDTO.ProductoId);
            if (!productoExists)
            {
                return BadRequest("El ProductoId proporcionado no es válido.");
            }

            var usuarioExists = await _context.User.AnyAsync(u => u.UserId == calificacionDTO.UserId);
            if (!usuarioExists)
            {
                return BadRequest("El UsuarioId proporcionado no es válido.");
            }

            try
            {
                var calificacion = new Calificaciones
                {
                    CalificacionId = Guid.NewGuid(),
                    ProductoId = calificacionDTO.ProductoId,
                    UserId = calificacionDTO.UserId,
                    Calificacion = calificacionDTO.Calificacion,
                    CalificacionTexto = calificacionDTO.CalificacionTexto,
                    FechaCalificacion = DateTime.UtcNow
                };

                // Guardar la calificación
                _context.Calificaciones.Add(calificacion);
                await _context.SaveChangesAsync();

                // Crear una entrada en ConjuntoCalificaciones
                var conjuntoCalificacion = new ConjuntoCalificaciones
                {
                    ProductoId = calificacionDTO.ProductoId,
                    CalificacionId = calificacion.CalificacionId
                };

                _context.ConjuntoCalificaciones.Add(conjuntoCalificacion);
                await _context.SaveChangesAsync();

                // Crear y retornar el DTO de respuesta
                var response = new CalificacionResponseDTO
                {
                    CalificacionId = calificacion.CalificacionId,
                    Calificacion = calificacion.Calificacion,
                    CalificacionTexto = calificacion.CalificacionTexto,
                    FechaCalificacion = calificacion.FechaCalificacion
                };

                return CreatedAtAction(nameof(GetCalificacionById), new { id = calificacion.CalificacionId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Error al agregar la calificación.",
                    Error = ex.Message
                });
            }
        }



        // Método GET para obtener todos los comentarios por ProductoId
        [HttpGet("porproduct/{productoId}")]
        public async Task<IActionResult> GetComentariosByProductoId(Guid productoId)
        {
            var comentarios = await _context.Calificaciones
                .Where(c => c.ProductoId == productoId) // Filtrar solo por productoId
                .ToListAsync();

            if (comentarios == null || !comentarios.Any())
            {
                return NotFound(new { Success = false, Message = "No se encontraron comentarios para este producto." });
            }

            // Crear y retornar una lista de DTOs para la respuesta
            var response = comentarios.Select(c => new CalificacionResponseDTO
            {
                CalificacionId = c.CalificacionId,
                ProductoId = c.ProductoId,
                UserId = c.UserId,
                Calificacion = c.Calificacion,
                CalificacionTexto = c.CalificacionTexto,
                FechaCalificacion = c.FechaCalificacion
            });

            return Ok(response);
        }




        // Método GET por ProductoId
        // Método para obtener comentarios por ProductoId y UserId
        [HttpGet("byproduct/{productoId}/byuser/{userId}")]
        public async Task<IActionResult> GetComentariosByProductoId(Guid productoId, Guid userId)
        {
            var comentarios = await _context.Calificaciones
                .Where(c => c.ProductoId == productoId && c.UserId == userId) // Filtrar por productoId y userId
                .ToListAsync();

            if (comentarios == null || !comentarios.Any())
            {
                return NotFound(new { Success = false, Message = "No se encontraron comentarios para este producto y usuario." });
            }

            return Ok(comentarios);
        }



        // Método para obtener una calificación por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCalificacionById(Guid id)
        {
            var calificacion = await _context.Calificaciones.FindAsync(id);
            if (calificacion == null)
            {
                return NotFound();
            }

            // Crear el DTO para la respuesta
            var response = new CalificacionResponseDTO
            {
                CalificacionId = calificacion.CalificacionId,
                ProductoId = calificacion.ProductoId,
                UserId = calificacion.UserId,
                Calificacion = calificacion.Calificacion,
                CalificacionTexto = calificacion.CalificacionTexto,
                FechaCalificacion = calificacion.FechaCalificacion
            };

            return Ok(response);
        }

    }
}
