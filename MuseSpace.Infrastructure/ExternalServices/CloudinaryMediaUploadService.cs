using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using MuseSpace.Core.Interfaces.Services;

namespace MuseSpace.Infrastructure.ExternalServices;

public class CloudinaryMediaUploadService : IMediaUploadService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryMediaUploadService(IConfiguration configuration)
    {
        var cloudinarySection = configuration.GetSection("Cloudinary");
        var account = new Account(
            cloudinarySection["CloudName"],
            cloudinarySection["ApiKey"],
            cloudinarySection["ApiSecret"]);

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<MediaUploadResult> UploadImageAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        if (stream.Length == 0)
        {
            throw new ArgumentException("Stream is empty", nameof(stream));
        }

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            Transformation = new Transformation().Width(1920).Crop("limit").Quality("auto").FetchFormat("auto"),
            Folder = "musespace/artworks/images"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Failed to upload image to Cloudinary: {uploadResult.Error.Message}");
        }

        // Generate thumbnail
        var thumbTransformation = new Transformation().Width(400).Crop("scale").Quality("auto").FetchFormat("auto");
        var thumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(thumbTransformation).BuildUrl(uploadResult.PublicId);

        return new MediaUploadResult
        {
            Url = uploadResult.SecureUrl.ToString(),
            ThumbnailUrl = thumbnailUrl,
            PublicId = uploadResult.PublicId,
            MediaType = "Image"
        };
    }

    public async Task<MediaUploadResult> UploadVideoAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        if (stream.Length == 0)
        {
            throw new ArgumentException("Stream is empty", nameof(stream));
        }

        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = "musespace/artworks/videos",
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Failed to upload video to Cloudinary: {uploadResult.Error.Message}");
        }

        // Generate thumbnail from middle of video
        var thumbTransformation = new Transformation().Width(400).Crop("scale").StartOffset("auto").FetchFormat("jpg");
        var thumbnailUrl = _cloudinary.Api.UrlVideoUp.Transform(thumbTransformation).BuildUrl(uploadResult.PublicId);

        return new MediaUploadResult
        {
            Url = uploadResult.SecureUrl.ToString(),
            ThumbnailUrl = thumbnailUrl,
            PublicId = uploadResult.PublicId,
            MediaType = "Video"
        };
    }

    public async Task<bool> DeleteMediaAsync(string publicId, CancellationToken cancellationToken = default)
    {
        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);
        return result.Result == "ok";
    }
}
