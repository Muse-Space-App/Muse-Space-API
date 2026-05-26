namespace MuseSpace.BLL.DTO;

public class NotificationResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string? ActionUrl { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    // Optional related data
    public int? RelatedUserId { get; set; }
    public string? RelatedUserUsername { get; set; }
    public string? RelatedUserAvatarUrl { get; set; }

    public int? RelatedArtworkId { get; set; }
    public string? RelatedArtworkThumbnailUrl { get; set; }
}
