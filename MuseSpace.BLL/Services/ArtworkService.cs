using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Interfaces.Services;
using MuseSpace.Core.Results;
using MuseSpace.Core.Enums;

namespace MuseSpace.BLL.Services;

public class ArtworkService : IArtworkService
{
    private readonly IArtworkRepository _artworkRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IMediaUploadService _mediaUploadService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public ArtworkService(
        IArtworkRepository artworkRepository,
        ITagRepository tagRepository,
        IMediaUploadService mediaUploadService,
        IUserRepository userRepository,
        IMapper mapper,
        IMemoryCache cache)
    {
        _artworkRepository = artworkRepository;
        _tagRepository = tagRepository;
        _mediaUploadService = mediaUploadService;
        _userRepository = userRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<GenericResult<ArtworkResponse>> CreateAsync(int userId, CreateArtworkRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return GenericResult<ArtworkResponse>.Failure("User not found", ErrorType.NotFound);
        }

        // Upload media
        MediaUploadResult uploadResult;
        try
        {
            var isVideo = request.MediaFile.ContentType.StartsWith("video/");
            using var stream = request.MediaFile.OpenReadStream();
            if (isVideo)
            {
                uploadResult = await _mediaUploadService.UploadVideoAsync(stream, request.MediaFile.FileName, cancellationToken);
            }
            else
            {
                uploadResult = await _mediaUploadService.UploadImageAsync(stream, request.MediaFile.FileName, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            return GenericResult<ArtworkResponse>.Failure($"Media upload failed: {ex.Message}", ErrorType.ValidationFailed);
        }

        var artwork = new Artwork
        {
            CreatorId = userId,
            Title = request.Title,
            Description = request.Description,
            ContentUrl = uploadResult.Url,
            ThumbnailUrl = uploadResult.ThumbnailUrl,
            MediaType = uploadResult.MediaType,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = user.Email
        };

        // Process tags
        var distinctTags = request.Tags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).Distinct().ToList();
        foreach (var tagName in distinctTags)
        {
            var tag = await _tagRepository.GetOrCreateAsync(tagName, user.Email, cancellationToken);
            artwork.ArtworkTags.Add(new ArtworkTag { TagId = tag.Id });
            await _tagRepository.IncrementUsageCountAsync(tag.Id, cancellationToken);
        }

        await _artworkRepository.AddAsync(artwork, cancellationToken);

        var response = _mapper.Map<ArtworkResponse>(artwork);
        return GenericResult<ArtworkResponse>.Success(response, "Artwork created successfully");
    }

    public async Task<GenericResult<ArtworkResponse>> GetByIdAsync(int id, int? currentUserId = null, CancellationToken cancellationToken = default)
    {
        // First get without tracking tags just to check existence
        var artwork = await _artworkRepository.GetByIdAsync(id, cancellationToken);
        if (artwork == null || artwork.IsSoftDeleted)
        {
            return GenericResult<ArtworkResponse>.Failure("Artwork not found", ErrorType.NotFound);
        }

        // Ideally we fetch a fully populated entity here.
        // We will just fetch it via the search logic to get includes.
        var fullArtwork = (await _artworkRepository.SearchAsync(artwork.Title, 0, 1, cancellationToken)).FirstOrDefault(a => a.Id == id);

        if (fullArtwork == null)
        {
            return GenericResult<ArtworkResponse>.Failure("Artwork not found", ErrorType.NotFound);
        }

        await _artworkRepository.IncrementViewCountAsync(id, cancellationToken);
        fullArtwork.ViewCount++;

        var response = _mapper.Map<ArtworkResponse>(fullArtwork);

        // TODO: Map interaction flags if currentUserId is present (IsLiked, IsBookmarked, IsFollowingCreator)

        return GenericResult<ArtworkResponse>.Success(response);
    }

    public async Task<GenericResult<PagedResult<ArtworkResponse>>> GetByCreatorIdAsync(int creatorId, int page, int pageSize, int? currentUserId = null, CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var artworks = await _artworkRepository.GetByCreatorIdAsync(creatorId, skip, pageSize, cancellationToken);

        var items = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(artworks);

        var pagedResult = new PagedResult<ArtworkResponse>
        {
            Items = items.ToList(),
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 0 // In a real app we would do a separate count query
        };

        return GenericResult<PagedResult<ArtworkResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<ArtworkResponse>> UpdateAsync(int id, int userId, UpdateArtworkRequest request, CancellationToken cancellationToken = default)
    {
        var artwork = await _artworkRepository.GetByIdAsync(id, cancellationToken);
        if (artwork == null || artwork.IsSoftDeleted)
        {
            return GenericResult<ArtworkResponse>.Failure("Artwork not found", ErrorType.NotFound);
        }

        if (artwork.CreatorId != userId)
        {
            return GenericResult<ArtworkResponse>.Failure("Only the creator can update this artwork", ErrorType.Unauthorized);
        }

        artwork.Title = request.Title;
        artwork.Description = request.Description;
        // Updating tags requires more logic (removing old, adding new). Simplifying for now.

        await _artworkRepository.UpdateAsync(artwork, cancellationToken);

        var response = _mapper.Map<ArtworkResponse>(artwork);
        return GenericResult<ArtworkResponse>.Success(response, "Artwork updated successfully");
    }

    public async Task<GenericResult<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var artwork = await _artworkRepository.GetByIdAsync(id, cancellationToken);
        if (artwork == null || artwork.IsSoftDeleted)
        {
            return GenericResult<bool>.Failure("Artwork not found", ErrorType.NotFound);
        }

        if (artwork.CreatorId != userId)
        {
            return GenericResult<bool>.Failure("Only the creator can delete this artwork", ErrorType.Unauthorized);
        }

        artwork.IsSoftDeleted = true;
        artwork.DeletedAtUtc = DateTime.UtcNow;
        await _artworkRepository.UpdateAsync(artwork, cancellationToken);

        return GenericResult<bool>.Success(true, "Artwork deleted successfully");
    }

    public async Task<GenericResult<PagedResult<ArtworkResponse>>> SearchAsync(string query, int page, int pageSize, int? currentUserId = null, CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var artworks = await _artworkRepository.SearchAsync(query, skip, pageSize, cancellationToken);

        var items = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(artworks);

        var pagedResult = new PagedResult<ArtworkResponse>
        {
            Items = items.ToList(),
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 0
        };

        return GenericResult<PagedResult<ArtworkResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<ArtworkFeedResponse>> GetFeedAsync(int? cursor, int limit, int? currentUserId = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"feed_{cursor}_{limit}";
        if (_cache.TryGetValue(cacheKey, out ArtworkFeedResponse? cachedResult) && cachedResult != null)
        {
            return GenericResult<ArtworkFeedResponse>.Success(cachedResult);
        }

        var artworks = await _artworkRepository.GetFeedAsync(cursor, limit + 1, cancellationToken);

        var hasMore = artworks.Count > limit;
        var itemsToReturn = hasMore ? artworks.Take(limit).ToList() : artworks.ToList();

        var responseItems = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(itemsToReturn);

        var response = new ArtworkFeedResponse
        {
            Items = responseItems,
            NextCursor = hasMore ? itemsToReturn.Last().Id : null,
            HasMore = hasMore
        };

        return GenericResult<ArtworkFeedResponse>.Success(response);
    }

    public async Task<GenericResult<IReadOnlyCollection<ArtworkResponse>>> GetTrendingAsync(int limit, int? currentUserId = null, CancellationToken cancellationToken = default)
    {
        var artworks = await _artworkRepository.GetTrendingAsync(limit, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(artworks);
        return GenericResult<IReadOnlyCollection<ArtworkResponse>>.Success(items);
    }

    public async Task<GenericResult<PagedResult<ArtworkResponse>>> GetLikedArtworksAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var artworks = await _artworkRepository.GetLikedByUserIdAsync(userId, skip, pageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(artworks).ToList();
        
        foreach (var item in items)
        {
            item.IsLiked = true;
        }
        
        var pagedResult = new PagedResult<ArtworkResponse>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 0 // Optimization: we can just return 0 since frontend doesn't strictly use total pages for masonry yet
        };

        return GenericResult<PagedResult<ArtworkResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<PagedResult<ArtworkResponse>>> GetBookmarkedArtworksAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var artworks = await _artworkRepository.GetBookmarkedByUserIdAsync(userId, skip, pageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(artworks).ToList();
        
        foreach (var item in items)
        {
            item.IsBookmarked = true;
        }
        
        var pagedResult = new PagedResult<ArtworkResponse>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 0
        };

        return GenericResult<PagedResult<ArtworkResponse>>.Success(pagedResult);
    }
}
