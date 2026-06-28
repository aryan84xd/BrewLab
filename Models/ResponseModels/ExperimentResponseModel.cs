using BrewLab.Models.Enums;

namespace BrewLab.Models.ResponseModels
{
    public class ExperimentResponseModel : BaseResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal CoffeeWeight { get; set; }
        public decimal WaterWeight { get; set; }
        public decimal? WaterTemperature { get; set; }
        public string? GrindSetting { get; set; }
        public int BrewTime { get; set; }
        public string? Remark { get; set; }

        public int? Aroma { get; set; }
        public int? Acidity { get; set; }
        public int? Body { get; set; }
        public int? Sweetness { get; set; }
        public int? Bitterness { get; set; }
        public int? Aftertaste { get; set; }

        public ExtractionType? Extraction { get; set; }
        public int Overall { get; set; }

        public Guid CoffeeId { get; set; }
        public Guid BrewMethodId { get; set; }

        public List<ExperimentParameterResponseModel> Parameters { get; set; } = [];
    }
}
