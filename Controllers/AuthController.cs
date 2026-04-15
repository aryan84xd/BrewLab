using BrewLab.Models.DTOs.UserDTO;
using BrewLab.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrewLab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<DTOUserLoginResponse>> Register([FromBody] DTOUserRegisterRequest dto)
        {
            try
            {
                var response = await _authService.RegisterAsync(dto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<DTOUserLoginResponse>> Login([FromBody] DTOUserLoginRequest dto)
        {
            var response = await _authService.LoginAsync(dto);
            if (response is null)
                return Unauthorized(new { message = "Invalid credentials." });

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<object>> Me()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !Guid.TryParse(userId, out var id))
                return Unauthorized();

            var user = await _authService.GetUserByIdAsync(id);
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
