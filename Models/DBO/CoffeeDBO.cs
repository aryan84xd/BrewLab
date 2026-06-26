namespace BrewLab.Models.DBO
{
    public class CoffeeDBO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Roast { get; set; } = string.Empty;
        public string? Origin { get; set; }
        public string? TastingNotes { get; set; }
        public DateTime? RoastDate { get; set; }
        public string? Process { get; set; }
        public Guid UserId { get; set; }
    }
}
