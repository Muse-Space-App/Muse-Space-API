namespace MuseSpace.Core.Entities;

public sealed class EventRsvp
{
    public int EventId { get; set; }
    public int UserId { get; set; }
    public DateTime RsvpedAtUtc { get; set; } = DateTime.UtcNow;

    public Event? Event { get; set; }
    public User? User { get; set; }
}
