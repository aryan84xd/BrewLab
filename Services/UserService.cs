using BrewLab.Data.Dtos;
using BrewLab.Models.User;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BrewLab.Services
{
    public class UserService: IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserLoginResponse> RegisterAsync(UserRegisterModel request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email)) throw new Exception("User already exist");

            var user = new UserDto
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)

            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            return new UserLoginResponse
            {
                Email = user.Email,
                Name = user.Name,
                Token = GenerateJwtToken(user)
            };
        }
        public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request)
        {
            

        }
    }
