using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Response;
using MuseSpace.BLL.Services;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.BLL.Interfaces.Services;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Authentication and authorization endpoints for MuseSpace API.
/// Handles user registration, login, token refresh, and OTP verification.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private const string OtpDisabledMessage = "OTP system is disabled.";

    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IAuthResponseFactory _authResponseFactory;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Constructor for AuthController
    /// </summary>
    /// <param name="authService">Authentication service for handling auth logic</param>
    /// <param name="tokenService">Token service for generating and validating JWT tokens</param>
    /// <param name="userRepository">User repository for accessing user data</param>
    /// <param name="authResponseFactory">Factory for building auth responses with tokens and user data</param>
    /// <remarks>
    /// The constructor injects the necessary services and repositories required for authentication operations. The IAuthService is responsible for handling the core authentication logic such as registering users and validating credentials. The ITokenService is used for generating JWT access tokens and refresh tokens, as well as validating them. The IUserRepository allows the controller to access user data from the database when needed, such as during OTP generation or verification. This setup follows the dependency injection pattern, promoting loose coupling and easier testing.
    /// </remarks>
    public AuthController(
        IAuthService authService,
        ITokenService tokenService,
        IUserRepository userRepository,
        IAuthResponseFactory authResponseFactory)
    {
        _authService = authService;
        _tokenService = tokenService;
        _userRepository = userRepository;
        _authResponseFactory = authResponseFactory;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <remarks>
    /// Creates a new user account with email verification required.
    /// Password must be at least 8 characters long.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/auth/register
    ///     {
    ///       "email": "user@example.com",
    ///       "username": "john_doe",
    ///       "password": "SecurePass123!",
    ///       "confirmPassword": "SecurePass123!",
    ///       "firstName": "John",
    ///       "lastName": "Doe"
    ///     }
    /// </remarks>
    /// <param name="request">User registration details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with access token and user details</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Registration failed - invalid data or email already exists</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.RegisterAsync(
            request.Email,
            request.Username,
            request.Password,
            request.FirstName,
            request.LastName,
            cancellationToken);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                AuthErrorType.Conflict => Conflict(new AuthResponse { Success = false, Message = result.Message }),
                AuthErrorType.Validation => ValidationProblem(CreateValidationProblem(result.Message)),
                _ => BadRequest(new AuthResponse { Success = false, Message = result.Message })
            };
        }

        return Ok(_authResponseFactory.CreateAuthResponse(result.User!, result.Message));
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <remarks>
    /// Authenticates a user with their email and password credentials.
    /// Returns JWT access token and refresh token on successful login.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///       "email": "user@example.com",
    ///       "password": "SecurePass123!"
    ///     }
    /// </remarks>
    /// <param name="request">Login credentials (email and password)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with JWT token and user details</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid email or password</response>
    /// <response code="400">Bad request - invalid input</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                AuthErrorType.Unauthorized => Unauthorized(new AuthResponse { Success = false, Message = result.Message }),
                AuthErrorType.Validation => ValidationProblem(CreateValidationProblem(result.Message)),
                _ => BadRequest(new AuthResponse { Success = false, Message = result.Message })
            };
        }

        return Ok(_authResponseFactory.CreateAuthResponse(result.User!, result.Message, result.User!.RefreshToken));
    }

    /// <summary>
    /// Refresh JWT access token using a valid refresh token
    /// </summary>
    /// <remarks>
    /// Allows clients to obtain a new access token using a valid refresh token without requiring the user
    /// to re-authenticate. The refresh token must be valid and not expired.
    /// 
    /// Sample request:
    /// /     POST /api/auth/refresh
    ///    ///     {
    ///     "refreshToken": "valid-refresh-token"
    ///    ///     }
    /// </remarks>
    /// <param name="request">Refresh token request containing the refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access token and refresh token if the refresh token is valid</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid or expired refresh token</response>
    /// <response code="400">Bad request - invalid input</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (user == null || user.RefreshTokenExpiryUtc <= DateTime.UtcNow)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        var tokenInfo = _tokenService.GenerateAccessTokenWithExpiry(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryUtc = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return Ok(new TokenResponse
        {
            AccessToken = tokenInfo.AccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = tokenInfo.ExpiresAtUtc
        });
    }

    /// <summary>
    /// Change password (requires authentication)
    /// </summary>
    /// <remarks>
    /// Allows authenticated users to change their password by providing the current password and a new password.
    /// The new password must meet the minimum length requirement and match the confirmation password.
    /// 
    /// Sample request:
    /// /     POST /api/auth/change-password
    ///    ///     {
    ///    "currentPassword": "CurrentPass123!",
    ///    "newPassword": "NewSecurePass456!",
    ///    "confirmPassword": "NewSecurePass456!"
    ///    ///     }
    /// </remarks>
    /// <param name="request">Change password request containing current and new passwords</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message if password is changed successfully</returns>
    /// <response code="200">Password changed successfully</response>
    /// <response code="400">Bad request - invalid input or password mismatch</response>
    /// <response code="401">Unauthorized - user not authenticated</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new AuthResponse { Success = false, Message = "User not found" });

        if (request.NewPassword != request.ConfirmPassword)
            return BadRequest(new AuthResponse { Success = false, Message = "Passwords do not match" });

        var result = await _authService.ChangePasswordAsync(
            userId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        if (result.Success)
            return Ok(new AuthResponse { Success = true, Message = result.Message });

        return result.ErrorType switch
        {
            AuthErrorType.Validation => ValidationProblem(CreateValidationProblem(result.Message)),
            AuthErrorType.NotFound => NotFound(new AuthResponse { Success = false, Message = result.Message }),
            _ => BadRequest(new AuthResponse { Success = false, Message = result.Message })
        };
    }

    private static ValidationProblemDetails CreateValidationProblem(string message)
    {
        return new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            ["validation"] = new[] { message }
        });
    }

    /// <summary>
    /// Generate OTP for email verification or password reset.
    /// </summary>
    /// <remarks>
    /// OTP is currently disabled. The legacy implementation is preserved in comments for future reactivation.
    /// </remarks>
    [HttpPost("otp/generate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OtpGenerateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OtpGenerateResponse>> GenerateOtp(
        [FromBody] OtpGenerateRequest request,
        CancellationToken cancellationToken = default)
    {
        /*
        var featureKey = request.Purpose switch
        {
            "EmailVerification" => "Features:OtpEmailVerification:Enabled",
            "ForgotPassword" => "Features:OtpForgotPassword:Enabled",
            _ => null
        };

        if (featureKey == null || !_configuration.GetValue<bool>(featureKey))
            return BadRequest(new OtpGenerateResponse
            {
                Success = false,
                Message = $"OTP for {request.Purpose} is not enabled"
            });

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return BadRequest(new OtpGenerateResponse
            {
                Success = false,
                Message = "User not found"
            });

        var otp = await _otpService.GenerateOtpAsync(user.Id, request.Purpose, cancellationToken);

        return Ok(new OtpGenerateResponse
        {
            Success = true,
            Message = $"OTP sent to {request.Email}",
            ExpiresIn = "15 minutes"
        });
        */

        return StatusCode(StatusCodes.Status503ServiceUnavailable, new OtpGenerateResponse
        {
            Success = false,
            Message = OtpDisabledMessage
        });
    }

    /// <summary>
    /// Verify OTP for email verification
    /// </summary>
    /// <remarks>
    /// OTP verification is currently disabled. The legacy implementation is preserved in comments for future reactivation.
    /// </remarks>
    [HttpPost("otp/verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OtpVerifyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OtpVerifyResponse>> VerifyEmailOtp(
        [FromBody] OtpVerifyRequest request,
        CancellationToken cancellationToken = default)
    {
        /*
        if (!_configuration.GetValue<bool>("Features:OtpEmailVerification:Enabled"))
            return BadRequest(new OtpVerifyResponse
            {
                Success = false,
                Message = "Email verification is not enabled"
            });

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return BadRequest(new OtpVerifyResponse
            {
                Success = false,
                Message = "User not found"
            });

        var (success, message) = await _otpService.VerifyOtpAsync(
            user.Id,
            request.Code,
            "EmailVerification",
            cancellationToken);

        if (!success)
            return BadRequest(new OtpVerifyResponse { Success = false, Message = message });

        // Mark email as verified
        user.IsEmailVerified = true;
        user.EmailVerifiedAtUtc = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return Ok(new OtpVerifyResponse
        {
            Success = true,
            Message = "Email verified successfully"
        });
        */

        return StatusCode(StatusCodes.Status503ServiceUnavailable, new OtpVerifyResponse
        {
            Success = false,
            Message = OtpDisabledMessage
        });
    }

    /// <summary>
    /// Request password reset OTP
    /// </summary>
    /// <remarks>
    /// Password reset via OTP is currently disabled. The legacy implementation is preserved in comments for future reactivation.
    /// </remarks>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OtpGenerateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OtpGenerateResponse>> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        /*
        if (!_configuration.GetValue<bool>("Features:OtpForgotPassword:Enabled"))
            return BadRequest(new OtpGenerateResponse
            {
                Success = false,
                Message = "Password reset is not enabled"
            });

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return Ok(new OtpGenerateResponse
            {
                Success = true,
                Message = "If the email exists, an OTP will be sent",
                ExpiresIn = "30 minutes"
            });

        await _otpService.GenerateOtpAsync(user.Id, "ForgotPassword", cancellationToken);

        return Ok(new OtpGenerateResponse
        {
            Success = true,
            Message = "Password reset OTP sent to your email",
            ExpiresIn = "30 minutes"
        });
        */

        return StatusCode(StatusCodes.Status503ServiceUnavailable, new OtpGenerateResponse
        {
            Success = false,
            Message = OtpDisabledMessage
        });
    }

    /// <summary>
    /// Reset password using OTP
    /// </summary>
    /// <remarks>
    /// Password reset via OTP is currently disabled. The legacy implementation is preserved in comments for future reactivation.
    /// </remarks>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        /*
        if (!_configuration.GetValue<bool>("Features:OtpForgotPassword:Enabled"))
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Password reset is not enabled"
            });

        if (request.NewPassword != request.ConfirmPassword)
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Passwords do not match"
            });

        if (request.NewPassword.Length < 8)
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Password must be at least 8 characters"
            });

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "User not found"
            });

        var (success, message) = await _otpService.VerifyOtpAsync(
            user.Id,
            request.Code,
            "ForgotPassword",
            cancellationToken);

        if (!success)
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = message
            });

        // Update password
        var passwordHasher = HttpContext.RequestServices.GetRequiredService<IPasswordHasher>();
        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiryUtc = null;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Password reset successfully"
        });
        */

        return StatusCode(StatusCodes.Status503ServiceUnavailable, new AuthResponse
        {
            Success = false,
            Message = OtpDisabledMessage
        });
    }
}
