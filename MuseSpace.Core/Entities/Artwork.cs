namespace MuseSpace.Core.Entities;

public sealed class Artwork : BaseEntity
{
    public int CreatorId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ContentUrl { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public string MediaType { get; set; } = string.Empty; // Image, GIF, Video

    public int ViewCount { get; set; } = 0;

    public int LikeCount { get; set; } = 0;

    public int CommentCount { get; set; } = 0;

    public bool IsApproved { get; set; } = false;

    public bool IsSoftDeleted { get; set; } = false;

    public DateTime? DeletedAtUtc { get; set; }

    public string? DeleteReason { get; set; }

    // Navigation properties
    public User? Creator { get; set; }
    public ICollection<ArtworkTag> ArtworkTags { get; set; } = new List<ArtworkTag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
