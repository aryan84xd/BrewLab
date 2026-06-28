using BCrypt.Net;

namespace BrewLab.Authentication
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
