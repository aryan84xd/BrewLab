using BrewLab.Models.ResponseModels;

public class ResponseFactory
{
    public T Failure<T>(string error, string message)
        where T : BaseResponse, new()
    {
        return new T
        {
            Success = false,
            Error = error,
            ErrorMessage = message
        };
    }
}