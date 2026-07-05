namespace BrewLab.Models.Entities
{
    public class Grinder
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Experiment> Experiments { get; set; } = [];
    }
}
