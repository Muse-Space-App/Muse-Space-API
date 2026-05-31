using MuseSpace.Core.Entities;

namespace MuseSpace.BLL.Interfaces.Services;

public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user. The token should include claims such as user ID, email, and roles, and be signed with a secure key. The token should also have an appropriate expiration time (e.g., 15 minutes) to balance security and usability.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a JWT access token and returns its expiration timestamp in UTC.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    (string AccessToken, DateTime ExpiresAtUtc) GenerateAccessTokenWithExpiry(User user);

    /// <summary>
    /// Generates a secure random refresh token. Refresh tokens are typically long-lived and used to obtain new access tokens without requiring the user to re-authenticate. The implementation should ensure that the generated token is sufficiently random and unique to prevent token prediction or reuse.
    /// </summary>
    /// <returns></returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT access token. This method should verify the token's signature, expiration, and any relevant claims to ensure that the token is valid and has not been tampered with. It should return true if the token is valid, and false otherwise.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Extracts the claims principal from an expired JWT access token. This is useful for scenarios where you want to allow users to refresh their tokens without requiring them to log in again, even if their access token has expired. The method should validate the token's signature and extract the claims, but it should not enforce the token's expiration time since the token is expected to be expired.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
