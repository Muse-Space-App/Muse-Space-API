using MuseSpace.Core.Enums;

namespace MuseSpace.Core.Results;

public class GenericResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public ErrorType ErrorType { get; private set; }

    private GenericResult() { }

    public static GenericResult<T> Success(T data, string message = "Success")
    {
        return new GenericResult<T> { IsSuccess = true, Data = data, Message = message, ErrorType = ErrorType.None };
    }

    public static GenericResult<T> Failure(string message, ErrorType errorType)
    {
        return new GenericResult<T> { IsSuccess = false, Message = message, ErrorType = errorType };
    }
}