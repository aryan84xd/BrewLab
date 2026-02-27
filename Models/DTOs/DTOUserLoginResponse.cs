namespace BrewLab.Models.DTOs
{
    public class DTOUserLoginResponse
    {
        public required string Token { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public DateTime ExpiresAtUtc { get; set;}
    }
}
