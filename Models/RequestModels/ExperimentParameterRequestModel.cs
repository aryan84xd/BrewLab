namespace BrewLab.Models.RequestModels
{
    public class ExperimentParameterRequestModel
    {
        public Guid BrewParameterId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
