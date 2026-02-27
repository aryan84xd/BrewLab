using BrewLab.Helpers;
using BrewLab.Models.DTOs;
using BrewLab.Models.Entities;
using BrewLab.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BrewLab.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtSettings _jwt;

        public AuthController(AppDbContext db, IOptions<JwtSettings> jwtOptions)
        {
            _db = db;
            _jwt = jwtOptions.Value;
        }

        [HttpPost("register")]
        public async Task<ActionResult<DTOUserLoginResponse>> Register([FromBody] DTOUserRegisterRequest dto)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return Conflict(new { message = "Email already registered." });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = JwtTokenHelper.GenerateToken(user, _jwt, out var expiresAt);
            return Ok(new DTOUserLoginResponse
            {
                Token = token,
                ExpiresAtUtc = expiresAt,
                Name = user.Name,
                Email = user.Email
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<DTOUserLoginResponse>> Login([FromBody] DTOUserLoginRequest dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null)
                return Unauthorized(new { message = "Invalid credentials." });

            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid)
                return Unauthorized(new { message = "Invalid credentials." });

            var token = JwtTokenHelper.GenerateToken(user, _jwt, out var expiresAt);
            return Ok(new DTOUserLoginResponse
            {
                Token = token,
                ExpiresAtUtc = expiresAt,
                Name = user.Name,
                Email = user.Email
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<object>> Me()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !Guid.TryParse(userId, out var id))
                return Unauthorized();

            var user = await _db.Users.FindAsync(id);
            if (user is null) return Unauthorized();

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email
            });
        }
    }
}

