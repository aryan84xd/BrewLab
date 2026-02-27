namespace BrewLab.Models.User
{
    public class UserRegisterModel
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
