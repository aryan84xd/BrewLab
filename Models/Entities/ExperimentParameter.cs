namespace BrewLab.Models.Entities
{
    public class ExperimentParameter
    {
        public Guid Id { get; set; }
        public Guid ExperimentId { get; set; }
        public Guid BrewParameterId { get; set; }

        public Experiment Experiment { get; set; } = null!;

        public BrewParameter BrewParameter { get; set; } = null!;

        public string Value { get; set; } = string.Empty;
    }
}
