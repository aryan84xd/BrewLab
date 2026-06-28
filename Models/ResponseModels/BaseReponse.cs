namespace BrewLab.Models.ResponseModels
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? ErrorMessage { get; set; }
    }
}