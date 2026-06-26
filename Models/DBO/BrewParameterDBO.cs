namespace BrewLab.Models.DBO
{
    public class BrewParameterDBO
    {
        public Guid Id { get; set; }
        public Guid BrewMethodId { get; set; }
        public string? DataType { get; set; }
        public string Key { get; set; } =string.Empty
        public string Label { get; set; }=string.Empty
        public string? Unit { get; set; }
        public int DisplayOrder { get; set; }
        public string? Required { get; set; }
        public bool? IsSystem { get; set; }
    }
}
