namespace BrewLab.DomainModel.DBModels
{
    public class ExperimentDBO
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string? BrewMethod { get; set; }
        public decimal CoffeeWeight { get; set; }
        public decimal WaterWeight { get; set; }
        public TimeOnly BrewTime { get; set; }
        public string? Remark { get; set; }
        public int Aroma { get; set; }
        public int Acidity { get; set; }
        public int Body { get; set; }
        public int Overall { get; set; }
        public Guid CoffeeId { get; set; }
        public Guid UserId { get; set; }
    }
}
