using BrewLab.Models.Enums;


namespace BrewLab.Models.Entities
{
   
    public class BrewParameter
    {
        public Guid Id { get; set; }
        public Guid BrewMethodId { get; set; }
        public ParameterType DataType { get; set; }
      
        public string Label { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int DisplayOrder { get; set; }
        public bool Required { get; set; }
        public bool IsSystem { get; set; }

        public BrewMethod BrewMethod { get; set; } = null!;

        public ICollection<ExperimentParameter> ExperimentParameters { get; set; } = [];
    }
}
