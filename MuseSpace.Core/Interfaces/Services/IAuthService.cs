using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, User? User)> RegisterAsync(
        string email,
        string username,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message, User? User)> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> ValidateEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}
