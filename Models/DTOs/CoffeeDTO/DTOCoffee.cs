namespace BrewLab.Models.DTOs.CoffeeDTO
{
    public class DTOCoffee
    {
        
        public Guid? Id { get; set; } 
        public required string Name { get; set; }
        public required string Brand { get; set; }
        public required string Roast { get; set; }
        public string? Origin { get; set; }

        public string? TastingNotes { get; set; }
    }
}
