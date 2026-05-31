using System.ComponentModel.DataAnnotations;

namespace MuseSpace.BLL.DTO;

public class CreateEventRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    public string BannerUrl { get; set; } = string.Empty;

    [Required]
    public DateTime StartDateUtc { get; set; }

    [Required]
    public DateTime EndDateUtc { get; set; }

    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;

    public bool IsOnline { get; set; } = true;

    [MaxLength(500)]
    public string EventUrl { get; set; } = string.Empty;
}

public class UpdateEventRequest
{
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    public string BannerUrl { get; set; } = string.Empty;

    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }

    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;

    public bool IsOnline { get; set; }

    [MaxLength(500)]
    public string EventUrl { get; set; } = string.Empty;
}

public class EventResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BannerUrl { get; set; } = string.Empty;
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public string EventUrl { get; set; } = string.Empty;
    public int OrganizerId { get; set; }
    public string OrganizerUsername { get; set; } = string.Empty;
    public string OrganizerAvatarUrl { get; set; } = string.Empty;
    public int RsvpCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class EventRsvpResponse
{
    public int EventId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public DateTime RsvpedAtUtc { get; set; }
}
