namespace BrewLab.Models.ResponseModels
{
    public class UserResponseModel : BaseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
