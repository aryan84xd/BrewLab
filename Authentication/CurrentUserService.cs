using System.Security.Claims;

namespace BrewLab.Authentication
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Guid UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                return claim != null && Guid.TryParse(claim.Value, out var userId) ? userId : Guid.Empty;
            }
        }

        public string Email
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email);
                return claim?.Value ?? string.Empty;
            }
        }

        public string Name
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name);
                return claim?.Value ?? string.Empty;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
            }
        }
    }
}
