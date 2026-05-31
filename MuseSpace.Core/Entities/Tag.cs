namespace MuseSpace.Core.Entities;

public sealed class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // For URL-friendly queries
    public int UsageCount { get; set; } = 0;
    public bool IsModerated { get; set; } = false;
    public bool IsBanned { get; set; } = false;
    public ICollection<ArtworkTag> ArtworkTags { get; set; } = new List<ArtworkTag>();
}
