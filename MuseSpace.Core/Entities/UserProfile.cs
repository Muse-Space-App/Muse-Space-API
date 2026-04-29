namespace MuseSpace.Core.Entities;

public sealed class UserProfile : BaseEntity
{
    public int UserId { get; set; }

    public string Bio { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string? BannerUrl { get; set; }

    public int FollowersCount { get; set; } = 0;

    public int FollowingCount { get; set; } = 0;

    public int ArtworkCount { get; set; } = 0;

    public string? SocialLinks { get; set; } // JSON

    public bool IsVerifiedCreator { get; set; } = false;

    public string? CreatorTier { get; set; } // Bronze, Silver, Gold, Platinum

    // Navigation properties
    public User? User { get; set; }
}
