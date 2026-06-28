using BrewLab.Models.Enums;

namespace BrewLab.Models.RequestModels
{
    public class CreateCoffeeRequestModel
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public RoastLevel Roast { get; set; }
        public string? Origin { get; set; }
        public string? TastingNotes { get; set; }
        public DateTime? RoastDate { get; set; }
        public ProcessType? Process { get; set; }
    }
}
