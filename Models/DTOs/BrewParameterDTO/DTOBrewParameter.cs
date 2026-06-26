using System.ComponentModel.DataAnnotations;

namespace BrewLab.Models.DTOs.BrewParameterDTO
{
    public class DTOBrewParameter
    {
        public Guid Id { get; set; }
        public Guid BrewMethodId { get; set; }
        public string? DataType { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public string? Unit { get; set; }
        public int DisplayOrder { get; set; }
        public string? Required { get; set; }
        public bool? IsSystem { get; set; }
    }
}
