using BrewLab.Models.Enums;


namespace BrewLab.Models.Entities
{


    public class Experiment
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
        public int? Overall { get; set; }
        public Guid CoffeeId { get; set; }
        public Guid BrewMethodId { get; set; }
        public Guid UserId { get; set; }

        public Coffee Coffee { get; set; } = null!;

        public BrewMethod BrewMethod { get; set; } = null!;

        public User User { get; set; } = null!;


        public Guid? GrinderId { get; set; }
        public Grinder? Grinder { get; set; }

        public Guid? BrewerId { get; set; }
        public Brewer? Brewer { get; set; }

        public ICollection<ExperimentParameter> Parameters { get; set; } = [];

    }
}
