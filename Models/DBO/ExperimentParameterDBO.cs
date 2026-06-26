namespace BrewLab.Models.DBO
{
    public class ExperimentParameterDBO
    {
        public Guid Id { get; set; }
        public Guid ExperimentId { get; set; }
        public Guid BrewParameterId { get; set; }
        public string Value { get; set; }=string.Empty;
    }
}
