using EduHuan.backend.DTOs;
using EduHuan.backend.Repositories;
using EduHuan.backend.Repositories.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHuan.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _uploadFolder;

        public ProductoController(AppDbContext context, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            if (env.ContentRootPath == null)
            {
                throw new ArgumentNullException(nameof(env.ContentRootPath), "El ContentRootPath no puede ser nulo.");
            }

            _uploadFolder = Path.Combine(env.ContentRootPath, "Imagenes");

            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        [HttpPost("uploadbookimage")]
        public async Task<IActionResult> UploadBookImage([FromForm] ProductoImageDTO productoImageDTO)
        {
            // Validar que solo uno de los dos campos esté presente, pero no ambos.
            if (productoImageDTO.ImageFile != null && !string.IsNullOrEmpty(productoImageDTO.ImageUrl))
            {
                return BadRequest(new { Success = false, Message = "Solo puede proporcionar un archivo o una URL, no ambos." });
            }

            // Validar que al menos uno de los dos esté presente.
            if (productoImageDTO.ImageFile == null && string.IsNullOrEmpty(productoImageDTO.ImageUrl))
            {
                return BadRequest(new { Success = false, Message = "Debe proporcionar una imagen o una URL." });
            }

            try
            {
                string fileName = string.Empty;

                // Si se proporciona un archivo, lo guardamos localmente
                if (productoImageDTO.ImageFile != null)
                {
                    var extension = Path.GetExtension(productoImageDTO.ImageFile.FileName);
                    fileName = Path.GetFileNameWithoutExtension(productoImageDTO.ImageFile.FileName) + extension;
                    var imagePath = Path.Combine(_uploadFolder, fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await productoImageDTO.ImageFile.CopyToAsync(stream);
                    }
                }
                // Si se proporciona una URL, la descargamos y la guardamos localmente
                else if (!string.IsNullOrEmpty(productoImageDTO.ImageUrl))
                {
                    var uri = new Uri(productoImageDTO.ImageUrl);
                    fileName = Path.GetFileName(uri.LocalPath);
                    var imagePath = Path.Combine(_uploadFolder, fileName);

                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(uri);

                        if (!response.IsSuccessStatusCode)
                        {
                            return BadRequest(new { Success = false, Message = "No se pudo descargar la imagen desde la URL proporcionada." });
                        }

                        var imageBytes = await response.Content.ReadAsByteArrayAsync();
                        await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                    }
                }

                return Ok(new { Success = true, FileName = fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Error al cargar la imagen.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("addproducto")]
        public async Task<IActionResult> AddProductoData(ProductoDTO productoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Datos no válidos.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {

                var producto = new Producto
                {
                    ProductoId = Guid.NewGuid(),
                    NombreProducto = productoDTO.NombreProducto,
                    Precio = productoDTO.Precio,
                    Descripcion = productoDTO.Descripcion,
                    Imagen = productoDTO.Imagen,
                    Categoria = productoDTO.Categoria,
                };

                _context.Producto.Add(producto);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Datos del libro registrados.", ProductoId = producto.ProductoId });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Error al agregar el libro.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("productoimage/{fileName}")]
        public IActionResult GetProductoImage(string fileName)
        {
            var filePath = Path.Combine(_uploadFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { Success = false, Message = "Imagen no encontrada." });
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            string contentType = fileExtension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };

            return File(fileBytes, contentType);
        }



        private string GetFileNameFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            // Extrae el nombre del archivo de la URL
            return url.Substring(url.LastIndexOf('/') + 1);
        }

        [HttpGet("productos")]
        public async Task<IActionResult> GetAllProductos()
        {
            try
            {
                var productos = await _context.Producto.ToListAsync();

                var productoDto = productos.Select(u => new ProductoDTO
                {
                    ProductoId = u.ProductoId,
                    NombreProducto = u.NombreProducto,
                    Precio = u.Precio,
                    Descripcion = u.Descripcion,
                    Imagen = GetFileNameFromUrl(u.Imagen),
                    Categoria = u.Categoria,
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    productos = productoDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Se produjo un error al obtener los productos.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("producto/{id}")]
        public async Task<IActionResult> GetProductoById(Guid id)
        {
            try
            {
                var producto = await _context.Producto.FindAsync(id);

                if (producto == null)
                {
                    return NotFound(new { Success = false, Message = "Producto no encontrado." });
                }

                var productoDto = new ProductoDTO
                {
                    ProductoId = producto.ProductoId,
                    NombreProducto = producto.NombreProducto,
                    Precio = producto.Precio,
                    Descripcion = producto.Descripcion,
                    Imagen = producto.Imagen,
                    Categoria= producto.Categoria,
                };

                return Ok(new { Success = true, Producto = productoDto });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Se produjo un error al obtener el producto.",
                    Error = ex.Message
                });
            }


        }
    }
}