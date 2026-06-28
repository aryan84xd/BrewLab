using BrewLab.Authentication;
using BrewLab.Models.Entities;
using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;
using BrewLab.Repositories.Interfaces;
using BrewLab.Services.Interfaces;


namespace BrewLab.Services.Implementations
{

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly ResponseFactory _responseFactory;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            JwtTokenGenerator jwtTokenGenerator,
            ResponseFactory responseFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
            _responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
        }

        public async Task<BaseResponse> RegisterAsync(RegisterRequestModel request)
        {
            if (request == null)
                return _responseFactory.Failure<BaseResponse>("InvalidRequest", "Registration request cannot be null");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.Name))
                return _responseFactory.Failure<BaseResponse>("InvalidName", "Name is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                return _responseFactory.Failure<BaseResponse>("InvalidEmail", "Email is required");

            if (string.IsNullOrWhiteSpace(request.Password))
                return _responseFactory.Failure<BaseResponse>("InvalidPassword", "Password is required");

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(request.Email))
                return _responseFactory.Failure<BaseResponse>("EmailAlreadyExists", "An account with this email already exists");

            try
            {
                // Hash password
                var passwordHash = _passwordHasher.HashPassword(request.Password);

                // Create user entity
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow
                };

                // Save to database
                await _userRepository.CreateAsync(user);

                return new BaseResponse { Success = true };
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<BaseResponse>("RegistrationFailed", $"An error occurred during registration: {ex.Message}");
            }
        }

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request)
        {
            var response = new LoginResponseModel();

            if (request == null)
                return _responseFactory.Failure<LoginResponseModel>("InvalidRequest", "Login request cannot be null");

            if (string.IsNullOrWhiteSpace(request.Email))
                return _responseFactory.Failure<LoginResponseModel>("InvalidEmail", "Email is required");

            if (string.IsNullOrWhiteSpace(request.Password))
                return _responseFactory.Failure<LoginResponseModel>("InvalidPassword", "Password is required");

            try
            {
                // Find user by email
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                    return _responseFactory.Failure<LoginResponseModel>("InvalidCredentials", "Invalid email or password");

                // Verify password
                if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                    return _responseFactory.Failure<LoginResponseModel>("InvalidCredentials", "Invalid email or password");

                // Generate JWT token
                var token = _jwtTokenGenerator.GenerateAccessToken(user);

                response.Success = true;
                response.UserId = user.Id;
                response.Name = user.Name;
                response.Email = user.Email;
                response.Token = token;

                return response;
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<LoginResponseModel>("LoginFailed", $"An error occurred during login: {ex.Message}");
            }
        }

        
    }
}
