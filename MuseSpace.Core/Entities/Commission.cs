using MuseSpace.Core.Enums;

namespace MuseSpace.Core.Entities;

public sealed class Commission : BaseEntity
{
    public int RequesterId { get; set; }
    public int ArtistId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CommissionStatus Status { get; set; } = CommissionStatus.Pending;

    public DateTime? DeadlineUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    public User? Requester { get; set; }
    public User? Artist { get; set; }

    public ICollection<CommissionMessage> Messages { get; set; } = new List<CommissionMessage>();
}
