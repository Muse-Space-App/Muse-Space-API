namespace MuseSpace.Core.Entities;

public sealed class Notification : BaseEntity
{
    public int UserId { get; set; }

    public string Type { get; set; } = string.Empty; // NewLike, NewComment, NewFollower, ArtworkApproved

    public int? RelatedUserId { get; set; }

    public int? RelatedArtworkId { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public string? ActionUrl { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public User? RelatedUser { get; set; }
    public Artwork? RelatedArtwork { get; set; }
}
