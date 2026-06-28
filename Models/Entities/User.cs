namespace BrewLab.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }


        // Navigation
        public ICollection<Coffee> Coffees { get; set; } = [];
        public ICollection<Experiment> Experiments { get; set; } = [];
    }
}
