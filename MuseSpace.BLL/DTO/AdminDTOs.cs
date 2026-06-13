using MuseSpace.BLL.Response;

namespace MuseSpace.BLL.DTO;

public class ReportResponse
{
    public int Id { get; set; }
    public int? ArtworkId { get; set; }
    public string ArtworkTitle { get; set; } = string.Empty;
    public int? TargetUserId { get; set; }
    public string TargetUsername { get; set; } = string.Empty;
    public int ReportedById { get; set; }
    public string ReportedByUsername { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? AdminNotes { get; set; }
}

public class BanUserRequest
{
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int? BanDurationDays { get; set; }
}

public class AdminStatsResponse
{
    public int TotalUsers { get; set; }
    public int TotalArtworks { get; set; }
    public int TotalReports { get; set; }
    public int PendingReports { get; set; }
    public int BannedUsers { get; set; }
}

public class ReviewReportRequest
{
    public string Status { get; set; } = string.Empty; // Approved, Rejected
    public string? AdminNotes { get; set; }
}
