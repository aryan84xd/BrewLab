namespace BrewLab.DomainModel.Contracts.ApiModels
{
    public class CreateCoffeeRequest
    {
        public required string Name { get; set; }
        public required string Brand { get; set; }
        public required string Roast { get; set; }
        public string? Origin { get; set; }
        public string? TastingNotes { get; set; }
    }
}
