namespace MuseSpace.Core.Entities;

public sealed class GroupPost : BaseEntity
{
    public int GroupId { get; set; }
    public int AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAtUtc { get; set; }

    public Group? Group { get; set; }
    public User? Author { get; set; }

    public ICollection<GroupPostLike> Likes { get; set; } = new List<GroupPostLike>();
    public ICollection<GroupPostComment> Comments { get; set; } = new List<GroupPostComment>();
}
