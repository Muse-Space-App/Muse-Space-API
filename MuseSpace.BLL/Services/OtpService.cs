using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace MuseSpace.BLL.Services;

public sealed class OtpService : IOtpService
{
    private readonly IOtpRepository _otpRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IConfiguration _configuration;
    private const int DefaultOtpExpirationMinutes = 15;
    private const int DefaultOtpLength = 6;
    private const int DefaultMaxAttempts = 3;

    public OtpService(
        IOtpRepository otpRepository,
        IDateTimeProvider dateTimeProvider,
        IConfiguration configuration)
    {
        _otpRepository = otpRepository;
        _dateTimeProvider = dateTimeProvider;
        _configuration = configuration;
    }

    public async Task<Otp> GenerateOtpAsync(
        int userId,
        string purpose,
        CancellationToken cancellationToken = default)
    {
        await InvalidateOtpsAsync(userId, purpose, cancellationToken);

        var expirationMinutes = GetConfigValue($"Features:Otp{purpose}:ExpirationMinutes", DefaultOtpExpirationMinutes);
        var code = GenerateRandomCode(OtpLength: DefaultOtpLength);
        var maxAttempts = GetConfigValue($"Features:Otp{purpose}:MaxAttempts", DefaultMaxAttempts);
        var now = _dateTimeProvider.UtcNow;

        var otp = new Otp
        {
            UserId = userId,
            Code = code,
            Purpose = purpose,
            ExpiresAtUtc = now.AddMinutes(expirationMinutes),
            IsUsed = false,
            AttemptsRemaining = maxAttempts,
            CreatedAtUtc = now,
            CreatedBy = $"OTP_{purpose}"
        };

        await _otpRepository.AddAsync(otp, cancellationToken);
        return otp;
    }

    public async Task<(bool Success, string Message)> VerifyOtpAsync(
        int userId,
        string code,
        string purpose,
        CancellationToken cancellationToken = default)
    {
        var otp = await _otpRepository.GetLatestValidOtpAsync(userId, purpose, cancellationToken);

        if (otp == null)
            return (false, "No valid OTP found");

        if (otp.IsUsed)
            return (false, "OTP has already been used");

        var now = _dateTimeProvider.UtcNow;
        if (now > otp.ExpiresAtUtc)
            return (false, "OTP has expired");

        if (otp.AttemptsRemaining <= 0)
        {
            otp.IsUsed = true;
            otp.UsedAtUtc = now;
            await _otpRepository.UpdateAsync(otp, cancellationToken);
            return (false, "Maximum verification attempts exceeded");
        }

        if (otp.Code != code)
        {
            otp.AttemptsRemaining--;
            await _otpRepository.UpdateAsync(otp, cancellationToken);
            return (false, $"Invalid OTP code. {otp.AttemptsRemaining} attempts remaining");
        }

        otp.IsUsed = true;
        otp.UsedAtUtc = now;
        await _otpRepository.UpdateAsync(otp, cancellationToken);

        return (true, "OTP verified successfully");
    }

    public async Task InvalidateOtpsAsync(
        int userId,
        string purpose,
        CancellationToken cancellationToken = default)
    {
        await _otpRepository.InvalidateUserOtpsAsync(userId, purpose, cancellationToken);
    }

    public async Task<Otp?> GetLatestValidOtpAsync(
        int userId,
        string purpose,
        CancellationToken cancellationToken = default)
    {
        var otp = await _otpRepository.GetLatestValidOtpAsync(userId, purpose, cancellationToken);

        if (otp == null)
            return null;

        var now = _dateTimeProvider.UtcNow;
        if (now > otp.ExpiresAtUtc || otp.IsUsed)
            return null;

        return otp;
    }

    private static string GenerateRandomCode(int OtpLength)
    {
        const string chars = "0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, OtpLength)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }

    private int GetConfigValue(string key, int defaultValue)
    {
        var value = _configuration[key];
        return int.TryParse(value, out var parsedValue) ? parsedValue : defaultValue;
    }
}
