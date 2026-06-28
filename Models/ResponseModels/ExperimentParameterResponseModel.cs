using BrewLab.Models.Enums;

namespace BrewLab.Models.ResponseModels
{
    public class ExperimentParameterResponseModel
    {
        public Guid BrewParameterId { get; set; }
        public string Label { get; set; } = string.Empty;
        public ParameterType DataType { get; set; }
        public string? Unit { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
