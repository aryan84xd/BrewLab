namespace BrewLab.Models.Entities
{
    public class Coffee
    {
        public  Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Brand { get; set; }
        public required string Roast { get; set; }
        public string? Origin { get; set; }
        public string? TastingNotes { get; set; }


        public User? User { get; set; }
        public Guid UserId { get; set; }

        public ICollection<Experiment>? Experiments { get; set; } = new List<Experiment>();
    }
}
