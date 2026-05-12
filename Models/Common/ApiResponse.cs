namespace BrewLab.Models.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                ErrorMessage = null,
                Data = data
            };
        }

        public static ApiResponse<T> FailureResponse(string errorMessage)
        {
            return new ApiResponse<T>
            {
                Success = false,
                ErrorMessage = errorMessage,
                Data = default
            };
        }
    }
}
