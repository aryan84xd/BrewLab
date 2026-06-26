namespace BrewLab.Models.Requests
{
    public class CreateCoffeeRequest
    {
        
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Roast { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string TastingNotes { get; set; } = string.Empty;
        public DateTime RoastDate { get; set; }
        public string Process { get; set; }=string.Empty;
        
    }
}
