using BrewLab.Models.Common;
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
        public async Task<ActionResult<ApiResponse<DTOUserLoginResponse>>> Register([FromBody] DTOUserRegisterRequest dto)
        {
            try
            {
                var response = await _authService.RegisterAsync(dto);
                return Ok(ApiResponse<DTOUserLoginResponse>.SuccessResponse(response));
            }
            catch (InvalidOperationException ex)
            {
                return Ok(ApiResponse<DTOUserLoginResponse>.FailureResponse(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<DTOUserLoginResponse>>> Login([FromBody] DTOUserLoginRequest dto)
        {
            var response = await _authService.LoginAsync(dto);
            if (response is null)
                return Ok(ApiResponse<DTOUserLoginResponse>.FailureResponse("Invalid credentials."));

            return Ok(ApiResponse<DTOUserLoginResponse>.SuccessResponse(response));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<object>>> Me()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !Guid.TryParse(userId, out var id))
                return Ok(ApiResponse<object>.FailureResponse("Unauthorized access."));

            var user = await _authService.GetUserByIdAsync(id);
            if (user is null) 
                return Ok(ApiResponse<object>.FailureResponse("User not found."));

            var userData = new
            {
                user.Id,
                user.Name,
                user.Email
            };

            return Ok(ApiResponse<object>.SuccessResponse(userData));
        }
    }
}
