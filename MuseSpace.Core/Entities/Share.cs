namespace MuseSpace.Core.Entities;

public sealed class Share : BaseEntity
{
    public int UserId { get; set; }
    public int ArtworkId { get; set; }
    public string? Platform { get; set; } // e.g., Twitter, Facebook, CopyLink
    public User? User { get; set; }
    public Artwork? Artwork { get; set; }
}
