using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.BLL.Mappings;
using MuseSpace.BLL.Helper;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Interfaces.Helper;

namespace MuseSpace.BLL.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHashHelper _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IRepository<Role> roleRepository,
        ITokenService tokenService,
        IPasswordHashHelper passwordHasher,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<AuthResult> RegisterAsync(
        string email,
        string username,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        if (!EmailValidationHelper.IsValidEmail(email))
            return AuthResult.Fail(AuthErrorType.Validation, "Invalid email format");

        if (await _userRepository.EmailExistsAsync(email, cancellationToken))
            return AuthResult.Fail(AuthErrorType.Conflict, "Email already registered");

        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        var memberRole = roles.FirstOrDefault(r => r.Name.ToUpper() == "MEMBER");

        if (memberRole == null)
            return AuthResult.Fail(AuthErrorType.NotFound, "Default Member role not found. Database initialization may have failed.");

        var user = new User
        {
            Email = email,
            Username = username,
            PasswordHash = _passwordHasher.HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            RoleId = memberRole.Id,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            CreatedBy = email,
            UserProfile = new UserProfile { Bio = string.Empty, AvatarUrl = string.Empty, BannerUrl = string.Empty, CreatorTier = string.Empty }
        };

        await _userRepository.AddAsync(user, cancellationToken);
        return AuthResult.Ok("Registration successful", user);
    }

    public Task<(bool Success, string Message)> ValidateEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var result = EmailValidationHelper.IsValidEmail(email)
            ? (true, "Email format is valid")
            : (false, "Invalid email format");

        return Task.FromResult(result);
    }

    public async Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user == null)
            return AuthResult.Fail(AuthErrorType.Unauthorized, "Invalid email or password");

        if (!user.IsActive)
            return AuthResult.Fail(AuthErrorType.Unauthorized, "User account is inactive");

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            return AuthResult.Fail(AuthErrorType.Unauthorized, "Invalid email or password");

        user.LastLoginUtc = _dateTimeProvider.UtcNow;
        user.RefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokenExpiryUtc = _dateTimeProvider.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user, cancellationToken);
        return AuthResult.Ok("Login successful", user);
    }

    public async Task<AuthActionResult> ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = (await _userRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return AuthActionResult.Fail(AuthErrorType.NotFound, "User not found");

        if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            return AuthActionResult.Fail(AuthErrorType.Validation, "Current password is incorrect");

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return AuthActionResult.Ok("Password changed successfully");
    }
}
