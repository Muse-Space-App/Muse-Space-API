using AutoMapper;
using BCrypt.Net;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Mappings;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Interfaces.Services;

namespace MuseSpace.BLL.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<(bool Success, string Message, User? User)> RegisterAsync(
        string email,
        string username,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        if (await _userRepository.EmailExistsAsync(email, cancellationToken))
            return (false, "Email already registered", null);

        var user = new User
        {
            Email = email,
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            CreatedBy = email
        };

        await _userRepository.AddAsync(user, cancellationToken);
        return (true, "Registration successful", user);
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user == null)
            return (false, "Invalid email or password", null);

        if (!user.IsActive)
            return (false, "User account is inactive", null);

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "Invalid email or password", null);

        user.LastLoginUtc = _dateTimeProvider.UtcNow;
        user.RefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokenExpiryUtc = _dateTimeProvider.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user, cancellationToken);
        return (true, "Login successful", user);
    }

    public Task<(bool Success, string Message)> ValidateEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var result = email.Contains('@') && email.Contains('.')
            ? (true, "Email format is valid")
            : (false, "Invalid email format");

        return Task.FromResult(result);
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = (await _userRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return (false, "User not found");

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return (false, "Current password is incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return (true, "Password changed successfully");
    }
}
