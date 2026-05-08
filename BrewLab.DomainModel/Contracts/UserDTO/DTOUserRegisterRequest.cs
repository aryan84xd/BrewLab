namespace BrewLab.DomainModel.Contracts.UserDTO
{
    public class DTOUserRegisterRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
