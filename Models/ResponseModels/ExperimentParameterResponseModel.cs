using BrewLab.Models.Enums;

namespace BrewLab.Models.ResponseModels
{
    public class ExperimentParameterResponseModel
    {
        public Guid BrewParameterId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
