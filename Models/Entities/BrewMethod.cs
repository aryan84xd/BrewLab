namespace BrewLab.Models.Entities
{
    public class BrewMethod
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<BrewParameter> BrewParameters { get; set; } = [];

        public ICollection<Experiment> Experiments { get; set; } = [];
    }
}
