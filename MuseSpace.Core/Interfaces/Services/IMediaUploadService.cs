namespace MuseSpace.Core.Interfaces.Services;

public class MediaUploadResult
{
    public string Url { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
}

public interface IMediaUploadService
{
    Task<MediaUploadResult> UploadImageAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
    Task<MediaUploadResult> UploadVideoAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
    Task<bool> DeleteMediaAsync(string publicId, CancellationToken cancellationToken = default);
}
