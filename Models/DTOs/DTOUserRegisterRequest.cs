namespace BrewLab.Models.DTOs
{
    public class DTOUserRegisterRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
