using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.Core.Entities;
using MuseSpace.BLL.Interfaces.Services;

namespace MuseSpace.BLL.Response;

public interface IAuthResponseFactory
{
    AuthResponse CreateAuthResponse(User user, string message, string? refreshTokenOverride = null);
}

public sealed class AuthResponseFactory : IAuthResponseFactory
{
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthResponseFactory(ITokenService tokenService, IMapper mapper)
    {
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public AuthResponse CreateAuthResponse(User user, string message, string? refreshTokenOverride = null)
    {
        var (accessToken, expiresAtUtc) = _tokenService.GenerateAccessTokenWithExpiry(user);
        var refreshToken = refreshTokenOverride ?? _tokenService.GenerateRefreshToken();

        return new AuthResponse
        {
            Success = true,
            Message = message,
            User = _mapper.Map<UserDto>(user),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAtUtc
        };
    }
}
