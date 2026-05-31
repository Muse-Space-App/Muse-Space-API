namespace MuseSpace.Core.Entities;

public sealed class Otp : BaseEntity
{
    public int UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty; // "EmailVerification", "ForgotPassword"
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAtUtc { get; set; }
    public int? AttemptsRemaining { get; set; } = 3;
    public User? User { get; set; }
}
