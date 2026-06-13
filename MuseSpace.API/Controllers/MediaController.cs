using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.Core.Interfaces.Services;
using MuseSpace.Core.Results;
using MuseSpace.Core.Enums;

namespace MuseSpace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaUploadService _mediaUploadService;

    public MediaController(IMediaUploadService mediaUploadService)
    {
        _mediaUploadService = mediaUploadService;
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadMedia(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(GenericResult<object>.Failure("No file provided", ErrorType.ValidationFailed));
        }

        try
        {
            using var stream = file.OpenReadStream();
            
            // Determine if it's a video or image based on content type
            var isVideo = file.ContentType.StartsWith("video/");
            
            MediaUploadResult result;
            if (isVideo)
            {
                result = await _mediaUploadService.UploadVideoAsync(stream, file.FileName, cancellationToken);
            }
            else
            {
                result = await _mediaUploadService.UploadImageAsync(stream, file.FileName, cancellationToken);
            }

            return Ok(GenericResult<MediaUploadResult>.Success(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, GenericResult<object>.Failure($"Upload failed: {ex.Message}", ErrorType.SystemError));
        }
    }
}
