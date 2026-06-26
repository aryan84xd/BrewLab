namespace BrewLab.Models.Requests
{
    public class CreateExperimentParameterReqeust
    {
        public Guid BrewParameterId { get; set; }
        public string Value { get; set; }
    }
}
