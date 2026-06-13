namespace MuseSpace.Core.Entities;

public sealed class GroupPostLike
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public int GroupPostId { get; set; }
    public GroupPost? GroupPost { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
