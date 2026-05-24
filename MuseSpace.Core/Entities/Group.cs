namespace MuseSpace.Core.Entities;

public sealed class Group : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string BannerUrl { get; set; } = string.Empty;
    public int CreatorId { get; set; }
    public bool IsPrivate { get; set; } = false;
    public int MemberCount { get; set; } = 0;

    public User? Creator { get; set; }
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<GroupPost> Posts { get; set; } = new List<GroupPost>();
}
