using Microsoft.AspNetCore.Mvc;
using BrewLab.Models.Common;

namespace BrewLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<object>> Get()
        {
            var healthInfo = new
            {
                status = "healthy",
                message = "BrewLab API is running with new architecture!",
                timestamp = DateTime.UtcNow,
                version = "2.0",
                architecture = "Request ? DTO ? DBO ? DB ? DBO ? DTO ? Response",
                features = new[]
                {
                    "All responses return HTTP 200",
                    "Success/Error fields in response body",
                    "Clean layer separation",
                    "No circular dependencies"
                }
            };

            return Ok(ApiResponse<object>.SuccessResponse(healthInfo));
        }

        [HttpGet("error-test")]
        public ActionResult<ApiResponse<object>> GetErrorTest()
        {
            return Ok(ApiResponse<object>.FailureResponse(
                "This is a test error message. Notice how it still returns HTTP 200!"));
        }

        [HttpGet("ping")]
        public ActionResult<ApiResponse<string>> Ping()
        {
            return Ok(ApiResponse<string>.SuccessResponse("pong"));
        }
    }
}
