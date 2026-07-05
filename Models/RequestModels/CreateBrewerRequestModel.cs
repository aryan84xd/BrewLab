namespace BrewLab.Models.RequestModels
{
    public class CreateBrewerRequestModel
    {
        public string Name { get; set; } = string.Empty;

        public Guid BrewMethodId { get; set; }
    }
}