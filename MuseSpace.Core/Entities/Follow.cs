namespace MuseSpace.Core.Entities;

public sealed class Follow
{
    public int FollowerId { get; set; }
    public int FollowingId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsBlocked { get; set; } = false;
    public DateTime? BlockedAtUtc { get; set; }
    public User? Follower { get; set; }
    public User? FollowingUser { get; set; }
}
