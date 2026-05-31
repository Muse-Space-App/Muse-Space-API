using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IArtworkRepository _artworkRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public RecommendationService(
        IArtworkRepository artworkRepository,
        IMapper mapper,
        IMemoryCache cache)
    {
        _artworkRepository = artworkRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<GenericResult<PagedResult<ArtworkResponse>>> GetRecommendedArtworksAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // Simple caching per user based on page
        var cacheKey = $"recommendations_{userId}_{page}_{pageSize}";

        if (_cache.TryGetValue(cacheKey, out PagedResult<ArtworkResponse>? cachedResult) && cachedResult != null)
        {
            return GenericResult<PagedResult<ArtworkResponse>>.Success(cachedResult);
        }

        // We use a relatively high limit for the repository, e.g., 100, then we paginate in memory.
        // This is a naive recommendation algorithm that doesn't scale perfectly to millions,
        // but works well for the project scope.
        int fetchLimit = 200;
        var artworks = await _artworkRepository.GetRecommendedAsync(userId, fetchLimit, cancellationToken);

        var totalCount = artworks.Count;
        var pagedArtworks = artworks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var artworkResponses = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(pagedArtworks);

        var pagedResult = new PagedResult<ArtworkResponse>
        {
            Items = artworkResponses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        // Cache the result for 5 minutes, giving them a static recommended list
        // and reducing db pressure.
        _cache.Set(cacheKey, pagedResult, TimeSpan.FromMinutes(5));

        return GenericResult<PagedResult<ArtworkResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<IReadOnlyCollection<ArtworkResponse>>> GetSimilarArtworksAsync(int artworkId, int limit, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"similar_artworks_{artworkId}_{limit}";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyCollection<ArtworkResponse>? cachedResult) && cachedResult != null)
        {
            return GenericResult<IReadOnlyCollection<ArtworkResponse>>.Success(cachedResult);
        }

        var targetArtwork = await _artworkRepository.GetByIdWithTagsAsync(artworkId, cancellationToken);
        if (targetArtwork == null)
            return GenericResult<IReadOnlyCollection<ArtworkResponse>>.Failure("Artwork not found", Core.Enums.ErrorType.NotFound);

        var targetTags = targetArtwork.ArtworkTags.Select(at => at.TagId).ToList();

        // Fetch candidates (e.g., up to 100 recent artworks) to score against
        var candidates = await _artworkRepository.GetFeedAsync(null, 100, cancellationToken);
        var filteredCandidates = candidates.Where(a => a.Id != artworkId).ToList();

        var targetTitleWords = ExtractKeywords(targetArtwork.Title);
        var targetDescWords = ExtractKeywords(targetArtwork.Description);
        var targetAiDescWords = ExtractKeywords(targetArtwork.AiDescription);

        var scoredCandidates = filteredCandidates.Select(candidate =>
        {
            int score = 0;

            var candidateTags = candidate.ArtworkTags.Select(at => at.TagId).ToList();
            score += candidateTags.Intersect(targetTags).Count() * 3;

            score += ExtractKeywords(candidate.Title).Intersect(targetTitleWords).Count() * 2;
            score += ExtractKeywords(candidate.Description).Intersect(targetDescWords).Count() * 1;
            score += ExtractKeywords(candidate.AiDescription).Intersect(targetAiDescWords).Count() * 1;

            return new { Artwork = candidate, Score = score };
        })
        .Where(x => x.Score > 0)
        .OrderByDescending(x => x.Score)
        .Take(limit)
        .Select(x => x.Artwork)
        .ToList();

        var responses = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(scoredCandidates);

        _cache.Set(cacheKey, responses, TimeSpan.FromMinutes(10));
        return GenericResult<IReadOnlyCollection<ArtworkResponse>>.Success(responses);
    }

    private HashSet<string> ExtractKeywords(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new HashSet<string>();

        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "is", "are", "was", "were", "in", "on", "at", "to", "for", "of",
            "and", "or", "but", "with", "by", "from", "this", "that", "it", "as", "be", "has",
            "have", "had", "not"
        };

        var punctuation = text.Where(char.IsPunctuation).Distinct().ToArray();
        var words = text.Split(punctuation).SelectMany(w => w.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        var keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var word in words)
        {
            if (!stopWords.Contains(word) && word.Length > 2)
            {
                keywords.Add(word.ToLowerInvariant());
            }
        }
        return keywords;
    }
}
