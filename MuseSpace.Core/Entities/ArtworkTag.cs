namespace MuseSpace.Core.Entities;

public sealed class ArtworkTag
{
    public int ArtworkId { get; set; }

    public int TagId { get; set; }

    // Navigation properties
    public Artwork? Artwork { get; set; }
    public Tag? Tag { get; set; }
}
