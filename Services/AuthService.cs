using BrewLab.Helpers;
using BrewLab.Models.DBO;
using BrewLab.Models.DTOs.UserDTO;
using BrewLab.Options;
using BrewLab.Repositories;

namespace BrewLab.Services
{
    public interface IAuthService
    {
        Task<DTOUserLoginResponse> RegisterAsync(DTOUserRegisterRequest request);
        Task<DTOUserLoginResponse?> LoginAsync(DTOUserLoginRequest request);
        Task<UserDBO?> GetUserByIdAsync(Guid id);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserRepository userRepository, JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings;
        }

        public async Task<DTOUserLoginResponse> RegisterAsync(DTOUserRegisterRequest request)
        {
            var exists = await _userRepository.ExistsByEmailAsync(request.Email);
            if (exists)
                throw new InvalidOperationException("Email already registered.");

            var user = new UserDBO
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.CreateAsync(user);

            var token = JwtTokenHelper.GenerateToken(user, _jwtSettings, out var expiresAt);
            return new DTOUserLoginResponse
            {
                Token = token,
                ExpiresAtUtc = expiresAt,
                Name = user.Name,
                Email = user.Email
            };
        }

        public async Task<DTOUserLoginResponse?> LoginAsync(DTOUserLoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                return null;

            var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!valid)
                return null;

            var token = JwtTokenHelper.GenerateToken(user, _jwtSettings, out var expiresAt);
            return new DTOUserLoginResponse
            {
                Token = token,
                ExpiresAtUtc = expiresAt,
                Name = user.Name,
                Email = user.Email
            };
        }

        public async Task<UserDBO?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
    }
}
