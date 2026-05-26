using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MuseSpace.BLL.Response;

namespace MuseSpace.BLL.DTO;

public class CreateArtworkRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public IFormFile MediaFile { get; set; } = null!;

    public List<string> Tags { get; set; } = new List<string>();
}

public class UpdateArtworkRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();
}

public class ArtworkResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ContentUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? AiDescription { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    // Creator info
    public int CreatorId { get; set; }
    public string CreatorUsername { get; set; } = string.Empty;
    public string CreatorProfileImageUrl { get; set; } = string.Empty;

    // Interaction flags
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
    public bool IsFollowingCreator { get; set; }

    public List<TagResponse> Tags { get; set; } = new List<TagResponse>();
}

public class ArtworkFeedResponse
{
    public IReadOnlyCollection<ArtworkResponse> Items { get; set; } = new List<ArtworkResponse>();
    public int? NextCursor { get; set; }
    public bool HasMore { get; set; }
}
