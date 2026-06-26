namespace BrewLab.Models.DTOs.ExperimentDTO
{
    public class DTOExperiment
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string? BrewMethod { get; set; }
        public decimal CoffeeWeight { get; set; }
        public decimal WaterWeight { get; set; }
        public string? WaterTemperature { get; set; }
        public string? GrindSetting { get; set; }
        public TimeOnly BrewTime { get; set; }
        public string? Remark { get; set; }
        public int Aroma { get; set; }
        public int Acidity { get; set; }
        public int Body { get; set; }
        public int Sweetness { get; set; }
        public int Bitterness { get; set; }
        public int Aftertaste { get; set; }
        public string? Extraction { get; set; }
        public int Overall { get; set; }
        public Guid CoffeeId { get; set; }
        public Guid BrewMethodId { get; set; }
        public Guid UserId { get; set; }
        public bool AdvanceOptions { get; set; }

    }
}
