using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.Core.Interfaces.Services;

namespace MuseSpace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService authService, ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(new AuthResponse { Success = false, Message = "Invalid request data" });

        if (request.Password != request.ConfirmPassword)
            return BadRequest(new AuthResponse { Success = false, Message = "Passwords do not match" });

        if (request.Password.Length < 8)
            return BadRequest(new AuthResponse { Success = false, Message = "Password must be at least 8 characters" });

        var (success, message, user) = await _authService.RegisterAsync(
            request.Email,
            request.Username,
            request.Password,
            request.FirstName,
            request.LastName,
            cancellationToken);

        if (!success)
            return BadRequest(new AuthResponse { Success = false, Message = message });

        var accessToken = _tokenService.GenerateAccessToken(user!);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return Ok(new AuthResponse
        {
            Success = true,
            Message = message,
            User = new UserDto
            {
                Id = user!.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified
            },
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(new AuthResponse { Success = false, Message = "Invalid request data" });

        var (success, message, user) = await _authService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!success)
            return Unauthorized(new AuthResponse { Success = false, Message = message });

        var accessToken = _tokenService.GenerateAccessToken(user!);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = message,
            User = new UserDto
            {
                Id = user!.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified,
                LastLoginUtc = user.LastLoginUtc
            },
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.RefreshToken);
        if (principal == null)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        return Ok(new TokenResponse
        {
            AccessToken = "new-access-token",
            RefreshToken = request.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    /// <summary>
    /// Change password (requires authentication)
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<AuthResponse>> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new AuthResponse { Success = false, Message = "User not found" });

        if (request.NewPassword != request.ConfirmPassword)
            return BadRequest(new AuthResponse { Success = false, Message = "Passwords do not match" });

        var (success, message) = await _authService.ChangePasswordAsync(
            userId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        return success
            ? Ok(new AuthResponse { Success = true, Message = message })
            : BadRequest(new AuthResponse { Success = false, Message = message });
    }

    /// <summary>
    /// Validate email format
    /// </summary>
    [HttpPost("validate-email")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> ValidateEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken = default)
    {
        var (success, message) = await _authService.ValidateEmailAsync(email, cancellationToken);
        return Ok(new AuthResponse { Success = success, Message = message });
    }
}
