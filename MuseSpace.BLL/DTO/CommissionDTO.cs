using MuseSpace.Core.Enums;

namespace MuseSpace.BLL.DTO;

public class CreateCommissionRequest
{
    public int ArtistId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime? DeadlineUtc { get; set; }
}

public class UpdateCommissionStatusRequest
{
    public CommissionStatus Status { get; set; }
    public string? ArtworkUrl { get; set; }
}

public class CommissionResponse
{
    public int Id { get; set; }
    public int RequesterId { get; set; }
    public string RequesterUsername { get; set; } = string.Empty;
    public string? RequesterAvatarUrl { get; set; }

    public int ArtistId { get; set; }
    public string ArtistUsername { get; set; } = string.Empty;
    public string? ArtistAvatarUrl { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CommissionStatus Status { get; set; }

    public DateTime? DeadlineUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ArtworkUrl { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CreateCommissionMessageRequest
{
    public string Content { get; set; } = string.Empty;

    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; }
}

public class CommissionMessageResponse
{
    public int Id { get; set; }
    public int CommissionId { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; } = string.Empty;
    public string? SenderAvatarUrl { get; set; }

    public string Content { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
