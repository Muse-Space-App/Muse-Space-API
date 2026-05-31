using System.ComponentModel.DataAnnotations;

namespace MuseSpace.BLL.DTO;

public class CreateCommentRequest
{
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    public int? ParentCommentId { get; set; }
}
