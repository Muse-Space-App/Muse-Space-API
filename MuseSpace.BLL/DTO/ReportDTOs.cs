using System.ComponentModel.DataAnnotations;

namespace MuseSpace.BLL.DTO;

public class CreateReportRequest
{
    public int? ArtworkId { get; set; }
    public int? TargetUserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ReportType { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;
}
