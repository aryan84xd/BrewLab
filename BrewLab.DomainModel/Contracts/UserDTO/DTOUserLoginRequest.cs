namespace BrewLab.DomainModel.Contracts.UserDTO
{
    public class DTOUserLoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
