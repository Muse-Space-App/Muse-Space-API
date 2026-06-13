using System.ComponentModel.DataAnnotations;

namespace MuseSpace.BLL.DTO;

public class CreateGroupRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public bool IsPrivate { get; set; } = false;

    public string? AvatarUrl { get; set; }
}

public class UpdateGroupRequest
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public bool IsPrivate { get; set; }
}

public class GroupResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string BannerUrl { get; set; } = string.Empty;
    public int CreatorId { get; set; }
    public bool IsPrivate { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsMember { get; set; }
}

public class GroupMemberResponse
{
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAtUtc { get; set; }
}

public class CreateGroupPostRequest
{
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
}

public class UpdateGroupPostRequest
{
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
}

public class GroupPostResponse
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
    public string AuthorAvatarUrl { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsEdited { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? EditedAtUtc { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsLiked { get; set; }
}
