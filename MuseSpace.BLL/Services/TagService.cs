using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public TagService(ITagRepository tagRepository, IMapper mapper, IMemoryCache cache)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<GenericResult<IReadOnlyCollection<TagResponse>>> GetPopularAsync(int limit, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"popular_tags_{limit}";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyCollection<TagResponse>? cachedResult) && cachedResult != null)
        {
            return GenericResult<IReadOnlyCollection<TagResponse>>.Success(cachedResult);
        }

        var tags = await _tagRepository.GetPopularAsync(limit, cancellationToken);
        var response = _mapper.Map<IReadOnlyCollection<TagResponse>>(tags);

        _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));

        return GenericResult<IReadOnlyCollection<TagResponse>>.Success(response);
    }

    public async Task<GenericResult<IReadOnlyCollection<TagResponse>>> GetByArtworkIdAsync(int artworkId, CancellationToken cancellationToken = default)
    {
        var tags = await _tagRepository.GetByArtworkIdAsync(artworkId, cancellationToken);
        var tagResponses = _mapper.Map<IReadOnlyCollection<TagResponse>>(tags);
        return GenericResult<IReadOnlyCollection<TagResponse>>.Success(tagResponses);
    }
}
