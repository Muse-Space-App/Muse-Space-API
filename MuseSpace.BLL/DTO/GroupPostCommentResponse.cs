namespace MuseSpace.BLL.DTO;

public class GroupPostCommentResponse
{
    public int Id { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
    public string AuthorAvatarUrl { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
