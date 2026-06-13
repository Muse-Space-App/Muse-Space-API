using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Services;

public class SearchService : ISearchService
{
    private readonly IArtworkRepository _artworkRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    private readonly MuseSpace.Infrastructure.Data.MuseSpaceDbContext _dbContext;

    public SearchService(
        IArtworkRepository artworkRepository,
        IUserRepository userRepository,
        ITagRepository tagRepository,
        IMapper mapper,
        MuseSpace.Infrastructure.Data.MuseSpaceDbContext dbContext)
    {
        _artworkRepository = artworkRepository;
        _userRepository = userRepository;
        _tagRepository = tagRepository;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GenericResult<SearchResponse>> SearchAsync(string? query, string? type, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            query = string.Empty;
        }

        int skip = (page - 1) * pageSize;
        var response = new SearchResponse();

        if (string.IsNullOrEmpty(type) || type.Equals("artwork", StringComparison.OrdinalIgnoreCase))
        {
            var artworks = await _artworkRepository.SearchAsync(query, skip, pageSize, cancellationToken);
            response.Artworks = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(artworks);
        }
        else
        {
            response.Artworks = new List<ArtworkResponse>();
        }

        if (string.IsNullOrEmpty(type) || type.Equals("user", StringComparison.OrdinalIgnoreCase))
        {
            var users = await _userRepository.SearchAsync(query, skip, pageSize, cancellationToken);
            response.Users = _mapper.Map<IReadOnlyCollection<UserProfileResponse>>(users);
        }
        else
        {
            response.Users = new List<UserProfileResponse>();
        }

        if (string.IsNullOrEmpty(type) || type.Equals("tag", StringComparison.OrdinalIgnoreCase))
        {
            var tags = await _tagRepository.SearchAsync(query, skip, pageSize, cancellationToken);
            response.Tags = _mapper.Map<IReadOnlyCollection<TagResponse>>(tags);
        }
        else
        {
            response.Tags = new List<TagResponse>();
        }

        return GenericResult<SearchResponse>.Success(response);
    }

    public async Task<GenericResult<PagedResult<ArtworkResponse>>> AdvancedSearchAsync(AdvancedSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<MuseSpace.Core.Entities.Artwork>()
            .Include(a => a.Creator)
            .Include(a => a.ArtworkTags)
            .ThenInclude(at => at.Tag)
            .Where(a => !a.IsSoftDeleted);

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var q = request.Query.ToLower();
            query = query.Where(a => a.Title.ToLower().Contains(q) || a.Description.ToLower().Contains(q));
        }

        if (!string.IsNullOrWhiteSpace(request.ExactTag))
        {
            var t = request.ExactTag.ToLower();
            query = query.Where(a => a.ArtworkTags.Any(at => at.Tag!.Slug == t || at.Tag.Name.ToLower() == t));
        }

        query = request.SortBy.ToLower() switch
        {
            "popular" => query.OrderByDescending(a => a.LikeCount).ThenByDescending(a => a.CreatedAtUtc),
            "views" => query.OrderByDescending(a => a.ViewCount).ThenByDescending(a => a.CreatedAtUtc),
            _ => query.OrderByDescending(a => a.CreatedAtUtc)
        };

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var responses = _mapper.Map<IReadOnlyCollection<ArtworkResponse>>(items);

        return GenericResult<PagedResult<ArtworkResponse>>.Success(new PagedResult<ArtworkResponse>
        {
            Items = responses,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        });
    }
}

