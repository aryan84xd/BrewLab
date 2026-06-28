using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;
using BrewLab.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrewLab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">Registration details (Name, Email, Password)</param>
        /// <returns>Success or error message</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(Register), result);
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials (Email, Password)</param>
        /// <returns>JWT token and user details on success</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponseModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LoginResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                if (result.Error == "InvalidCredentials")
                    return Unauthorized(result);

                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
