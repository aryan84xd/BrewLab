namespace BrewLab.Models.RequestModels
{
    public class UpdateBrewerRequestModel
    {
        public string Name { get; set; } = string.Empty;

        public Guid BrewMethodId { get; set; }
    }
}