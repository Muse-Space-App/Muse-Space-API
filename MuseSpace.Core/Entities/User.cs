namespace MuseSpace.Core.Entities;

public sealed class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool IsEmailVerified { get; set; } = false;

    public DateTime? LastLoginUtc { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryUtc { get; set; }

    public int RoleId { get; set; }

    public bool IsBanned { get; set; } = false;

    public string? BanReason { get; set; }

    public DateTime? BanExpiryUtc { get; set; }

    // Navigation properties
    public Role? Role { get; set; }
    public UserProfile? UserProfile { get; set; }
    public ICollection<Artwork> CreatedArtwork { get; set; } = new List<Artwork>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public ICollection<Follow> Following { get; set; } = new List<Follow>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
