using System.ComponentModel.DataAnnotations;

namespace MuseSpace.BLL.DTO;

public class UpdateCommentRequest
{
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
}
