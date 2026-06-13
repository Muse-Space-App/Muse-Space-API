namespace MuseSpace.Core.Entities;

public sealed class GroupPostComment : BaseEntity
{
    public int GroupPostId { get; set; }
    public GroupPost? GroupPost { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public string Content { get; set; } = string.Empty;
    public bool IsSoftDeleted { get; set; } = false;
}
