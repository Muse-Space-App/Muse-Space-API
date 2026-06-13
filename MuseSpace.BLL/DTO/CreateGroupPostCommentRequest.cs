using System.ComponentModel.DataAnnotations;

namespace MuseSpace.BLL.DTO;

public class CreateGroupPostCommentRequest
{
    [Required(ErrorMessage = "Content is required")]
    [MaxLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
    public string Content { get; set; } = string.Empty;
}
