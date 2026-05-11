using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Services;

public enum AuthErrorType
{
    None,
    Validation,
    Conflict,
    Unauthorized,
    NotFound
}

public sealed class AuthResult
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public User? User { get; init; }

    public AuthErrorType ErrorType { get; init; }

    public static AuthResult Ok(string message, User user) => new()
    {
        Success = true,
        Message = message,
        User = user,
        ErrorType = AuthErrorType.None
    };

    public static AuthResult Fail(AuthErrorType errorType, string message) => new()
    {
        Success = false,
        Message = message,
        ErrorType = errorType
    };
}

public sealed class AuthActionResult
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public AuthErrorType ErrorType { get; init; }

    public static AuthActionResult Ok(string message) => new()
    {
        Success = true,
        Message = message,
        ErrorType = AuthErrorType.None
    };

    public static AuthActionResult Fail(AuthErrorType errorType, string message) => new()
    {
        Success = false,
        Message = message,
        ErrorType = errorType
    };
}
