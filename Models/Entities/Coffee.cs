using BrewLab.Models.Enums;

namespace BrewLab.Models.Entities
{
    public class Coffee
    {
        public DateTime CreatedAt { get; set; }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public RoastLevel Roast { get; set; }
        public string? Origin { get; set; }
        public string? TastingNotes { get; set; }
        public DateTime? RoastDate { get; set; }
        public ProcessType? Process { get; set; }
        public decimal? Rating { get; set; }
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public ICollection<Experiment> Experiments { get; set; } = [];
    }
}
