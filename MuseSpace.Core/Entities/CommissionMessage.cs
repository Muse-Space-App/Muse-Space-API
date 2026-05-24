namespace MuseSpace.Core.Entities;

public sealed class CommissionMessage : BaseEntity
{
    public int CommissionId { get; set; }
    public int SenderId { get; set; }

    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;

    public Commission? Commission { get; set; }
    public User? Sender { get; set; }
}
