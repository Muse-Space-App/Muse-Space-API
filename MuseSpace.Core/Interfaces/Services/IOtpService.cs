using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Services;

public interface IOtpService
{
    /// <summary>
    /// Generates a new OTP code for the specified user and purpose. Invalidates any existing OTP
    /// for the user and purpose before creating a new one. Returns the generated OTP.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="purpose"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Otp> GenerateOtpAsync(int userId, string purpose, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verifies an OTP code
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <param name="purpose"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<(bool Success, string Message)> VerifyOtpAsync(
        int userId,
        string code,
        string purpose,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Invalidates all OTPs for a user and purpose. This is typically called before generating a new OTP to ensure only one valid OTP exists at a time.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="purpose"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InvalidateOtpsAsync(int userId, string purpose, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the latest valid OTP for a user and purpose
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="purpose"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Otp?> GetLatestValidOtpAsync(int userId, string purpose, CancellationToken cancellationToken = default);
}
