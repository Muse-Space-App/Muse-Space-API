namespace MuseSpace.Core.Entities;

public sealed class Event : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BannerUrl { get; set; } = string.Empty;
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool IsOnline { get; set; } = true;
    public string EventUrl { get; set; } = string.Empty;
    public int OrganizerId { get; set; }
    public int RsvpCount { get; set; } = 0;

    public User? Organizer { get; set; }
    public ICollection<EventRsvp> Rsvps { get; set; } = new List<EventRsvp>();
}
