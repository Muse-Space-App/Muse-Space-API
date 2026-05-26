using System.ComponentModel.DataAnnotations;

namespace MuseSpace.BLL.DTO;

public class ShareArtworkRequest
{
    [Required]
    [MaxLength(50)]
    public string Platform { get; set; } = string.Empty;
}
