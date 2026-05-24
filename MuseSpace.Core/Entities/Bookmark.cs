namespace MuseSpace.Core.Entities;

public sealed class Bookmark
{
    public int UserId { get; set; }
    public int ArtworkId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public User? User { get; set; }
    public Artwork? Artwork { get; set; }
}
