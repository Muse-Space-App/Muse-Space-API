namespace MuseSpace.BLL.DTO;

public class CommentResponse
{
    public int Id { get; set; }
    public int ArtworkId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string UserProfileImageUrl { get; set; } = string.Empty;
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsEdited { get; set; }
    public DateTime? EditedAtUtc { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
