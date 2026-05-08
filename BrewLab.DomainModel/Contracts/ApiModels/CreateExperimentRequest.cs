namespace BrewLab.DomainModel.Contracts.ApiModels
{
    public class CreateExperimentRequest
    {
        public Guid CoffeeId { get; set; }
        public string? BrewMethod { get; set; }
        public decimal CoffeeWeight { get; set; }
        public decimal WaterWeight { get; set; }
        public TimeOnly BrewTime { get; set; }
        public string? Remark { get; set; }
        public int Aroma { get; set; }
        public int Acidity { get; set; }
        public int Body { get; set; }
        public int Overall { get; set; }
    }
}
