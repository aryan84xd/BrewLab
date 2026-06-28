using BrewLab.Data;
using BrewLab.Models.Entities;
using BrewLab.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewLab.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
