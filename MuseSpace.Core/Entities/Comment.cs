namespace MuseSpace.Core.Entities;

public sealed class Comment : BaseEntity
{
    public int ArtworkId { get; set; }

    public int UserId { get; set; }

    public int? ParentCommentId { get; set; } // For threaded replies

    public string Content { get; set; } = string.Empty;

    public bool IsEdited { get; set; } = false;

    public DateTime? EditedAtUtc { get; set; }

    public int LikeCount { get; set; } = 0;

    public bool IsSoftDeleted { get; set; } = false;

    public DateTime? ApprovedAtUtc { get; set; }

    // Navigation properties
    public Artwork? Artwork { get; set; }
    public User? User { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
