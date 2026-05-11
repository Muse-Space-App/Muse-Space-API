namespace MuseSpace.Core.Entities;

public sealed class AuditLog : BaseEntity
{
    public int UserId { get; set; }
    public string EntityType { get; set; } = string.Empty; // Artwork, Comment, User, etc.
    public int EntityId { get; set; }
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Moderate
    public string? Changes { get; set; } // JSON payload
    public string? IpAddress { get; set; }
    public User? User { get; set; }
}
