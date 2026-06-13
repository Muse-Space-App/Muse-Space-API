namespace MuseSpace.Core.Entities;

public sealed class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; } = false;
    public DateTime? EmailVerifiedAtUtc { get; set; }
    public DateTime? LastLoginUtc { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryUtc { get; set; }
    public int RoleId { get; set; }
    public bool IsBanned { get; set; } = false;
    public string? BanReason { get; set; }
    public DateTime? BanExpiryUtc { get; set; }
    public Role? Role { get; set; }
    public UserProfile? UserProfile { get; set; }
    public ICollection<Artwork> CreatedArtwork { get; set; } = new List<Artwork>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    public ICollection<Share> Shares { get; set; } = new List<Share>();
    public ICollection<Group> CreatedGroups { get; set; } = new List<Group>();
    public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    public ICollection<GroupPost> GroupPosts { get; set; } = new List<GroupPost>();
    public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
    public ICollection<EventRsvp> EventRsvps { get; set; } = new List<EventRsvp>();
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public ICollection<Follow> Following { get; set; } = new List<Follow>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<Report> ReceivedReports { get; set; } = new List<Report>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<Commission> RequestedCommissions { get; set; } = new List<Commission>();
    public ICollection<Commission> ReceivedCommissions { get; set; } = new List<Commission>();
    public ICollection<CommissionMessage> CommissionMessages { get; set; } = new List<CommissionMessage>();

    public ICollection<GroupPostLike> GroupPostLikes { get; set; } = new List<GroupPostLike>();
    public ICollection<GroupPostComment> GroupPostComments { get; set; } = new List<GroupPostComment>();
}
