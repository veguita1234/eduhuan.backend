using EduHuan.backend.DTOs.Response;
using EduHuan.backend.DTOs;
using EduHuan.backend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EduHuan.backend.DTOs.Request;
using EduHuan.backend.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduHuan.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string Encripter(string texto)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] textoEnBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(texto));
                return string.Concat(textoEnBytes.Select(b => b.ToString("x2")));
            }
        }

        private LoginResponseDTO GenerateToken(UserDTO user)
        {
            var expires = DateTime.UtcNow.AddHours(16);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Name", user.Name ?? string.Empty),
                new Claim("LastName", user.LastName ?? string.Empty),
                new Claim("Tipo", user.Tipo ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var securityToken = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new LoginResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Name = user.Name,
                LastName = user.LastName,
                UserName = user.UserName
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginDto)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.UserName == loginDto.UserName && u.Tipo == loginDto.Tipo);

            if (user == null)
            {
                return Unauthorized(new { Success = false, Message = "Usuario o tipo incorrecto." });
            }

            if (user.Password != Encripter(loginDto.Password))
            {
                return Unauthorized(new { Success = false, Message = "Contraseña incorrecta." });
            }

            var userDto = new UserDTO
            {
                UserId = user.UserId,
                Tipo = user.Tipo,
                Name = user.Name,
                LastName = user.LastName,
                Celular = user.Celular,
                UserName = user.UserName,
                Password = user.Password
            };

            var tokenResponse = GenerateToken(userDto);

            return Ok(new
            {
                Success = true,
                Message = "Login exitoso.",
                Token = tokenResponse.Token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userDto)
        {
            if (await _context.User.AnyAsync(u => u.Name == userDto.Name))
                return BadRequest(new { Success = false, Field = "name", Message = "El nombre ya está en uso." });

            if (await _context.User.AnyAsync(u => u.Celular == userDto.Celular))
                return BadRequest(new { Success = false, Field = "email", Message = "El celular ya está en uso." });

            if (await _context.User.AnyAsync(u => u.UserName == userDto.UserName))
                return BadRequest(new { Success = false, Field = "userName", Message = "El nombre de usuario ya está en uso." });

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Tipo = userDto.Tipo,
                Name = userDto.Name,
                LastName = userDto.LastName,
                Celular = userDto.Celular,
                UserName = userDto.UserName,
                Password = Encripter(userDto.Password)
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Registro exitoso." });
        }




        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.User.ToListAsync();

                var userDtos = users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Tipo = u.Tipo,
                    Name = u.Name,
                    LastName = u.LastName,
                    Celular =u.Celular,
                    UserName = u.UserName
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Users = userDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Se produjo un error al obtener los usuarios.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("users/filter")]
        public async Task<IActionResult> GetUsersByUserName([FromQuery] string searchTerm)
        {
            try
            {
                var query = _context.User.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    // Convierte el término de búsqueda y el UserName a minúsculas para la comparación
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(u => u.UserName.ToLower().Contains(searchTerm));
                }

                var users = await query.ToListAsync();

                var userDtos = users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Tipo = u.Tipo,
                    Name = u.Name,
                    LastName = u.LastName,
                    Celular=u.Celular,
                    UserName = u.UserName
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Users = userDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Se produjo un error al obtener los usuarios filtrados.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("users/filter/byuserid")]
        public async Task<IActionResult> GetUsersByUserId([FromQuery] Guid? userId)
        {
            try
            {
                var query = _context.User.AsQueryable();

                // Verifica si se ha proporcionado un userId para filtrar
                if (userId.HasValue)
                {
                    query = query.Where(u => u.UserId == userId.Value);
                }

                var users = await query.ToListAsync();

                var userDtos = users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Tipo = u.Tipo,
                    Name = u.Name,
                    LastName = u.LastName,
                    Celular = u.Celular,
                    UserName = u.UserName
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Users = userDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Se produjo un error al obtener los usuarios filtrados por UserId.",
                    Error = ex.Message
                });
            }
        }


    }
}
