namespace MuseSpace.Core.Entities;

public sealed class Report : BaseEntity
{
    public int ReportedById { get; set; }
    public int ArtworkId { get; set; }
    public string ReportType { get; set; } = string.Empty; // Spam, Violence, Copyright, Inappropriate
    public string Reason { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Actioned
    public int? ResolutionId { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public int? ReviewedByAdminId { get; set; }
    public User? ReportedBy { get; set; }
    public Artwork? Artwork { get; set; }
    public User? ReviewedByAdmin { get; set; }
}
