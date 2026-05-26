using MuseSpace.Core.Entities;

namespace MuseSpace.BLL.Interfaces.Services;

public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the provided details. Returns success status, message, and the created user if successful.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AuthResult> RegisterAsync(
        string email,
        string username,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user with the provided email and password. Returns success status, message, and the user if authentication is successful.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the format of an email address. Returns success status and message.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<(bool Success, string Message)> ValidateEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the password for a user with the specified ID. Returns success status and message.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="currentPassword"></param>
    /// <param name="newPassword"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AuthActionResult> ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}
