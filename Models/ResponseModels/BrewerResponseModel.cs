namespace BrewLab.Models.ResponseModels
{
    public class BrewerResponseModel : BaseResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid BrewMethodId { get; set; }
    }
}