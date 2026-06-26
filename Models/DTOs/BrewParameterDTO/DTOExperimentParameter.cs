namespace BrewLab.Models.DTOs.BrewParameterDTO
{
    public class DTOExperimentParameter
    {
        public Guid Id { get; set; }
        public Guid ExperimentId { get; set; }
        public Guid BrewParameterId { get; set; }
        public string Value { get; set; }
    }
}
