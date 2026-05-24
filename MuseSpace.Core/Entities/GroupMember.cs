namespace MuseSpace.Core.Entities;

public sealed class GroupMember
{
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = "Member"; // Admin, Moderator, Member
    public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;

    public Group? Group { get; set; }
    public User? User { get; set; }
}
