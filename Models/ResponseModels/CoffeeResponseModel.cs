using BrewLab.Models.Enums;

namespace BrewLab.Models.ResponseModels
{
    public class CoffeeResponseModel : BaseResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public RoastLevel Roast { get; set; }
        public string? Origin { get; set; }
        public string? TastingNotes { get; set; }
        public DateTime? RoastDate { get; set; }
        public ProcessType? Process { get; set; }
    }
}
