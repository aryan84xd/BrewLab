namespace BrewLab.Models.User
{
    public class UserLoginResponse
    {
        public required string Token { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
    }
}
