namespace BrewLab.Data.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // auto-generate
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
