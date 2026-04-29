using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    bool ValidateToken(string token);

    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
