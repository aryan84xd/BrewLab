namespace BrewLab.Models.ResponseModels
{
    public class GrinderResponseModel : BaseResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}