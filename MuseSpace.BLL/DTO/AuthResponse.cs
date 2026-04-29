namespace MuseSpace.BLL.DTO;

public sealed class AuthResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public UserDto? User { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? ExpiresAt { get; set; }
}

public sealed class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}

public sealed class UserDto
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsEmailVerified { get; set; }

    public DateTime? LastLoginUtc { get; set; }
}
