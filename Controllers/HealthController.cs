using BrewLab.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrewLab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public HealthController(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Health check and keep-alive endpoint
        /// This endpoint can be called by monitoring services to ensure the backend and database are awake
        /// </summary>
        /// <returns>OK status with timestamp</returns>
        [HttpGet("welcome")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Welcome()
        {
            try
            {
                // Test database connectivity
                var canConnect = await _dbContext.Database.CanConnectAsync();

                if (!canConnect)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                        new HealthResponse 
                        { 
                            Status = "Unhealthy", 
                            Message = "Database connection failed",
                            Timestamp = DateTime.UtcNow
                        });
                }

                return Ok(new HealthResponse
                {
                    Status = "Healthy",
                    Message = "Backend and database are awake",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new HealthResponse
                    {
                        Status = "Unhealthy",
                        Message = $"Health check failed: {ex.Message}",
                        Timestamp = DateTime.UtcNow
                    });
            }
        }
    }

    /// <summary>
    /// Response model for health check endpoint
    /// </summary>
    public class HealthResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
