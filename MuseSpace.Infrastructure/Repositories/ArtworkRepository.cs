using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public class ArtworkRepository : Repository<Artwork>, IArtworkRepository
{
    private readonly MuseSpaceDbContext _dbContext;

    public ArtworkRepository(MuseSpaceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Artwork?> GetByIdWithTagsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Artwork>()
            .Include(a => a.Creator)
            .Include(a => a.ArtworkTags)
            .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsSoftDeleted, cancellationToken);
    }
    public async Task<IReadOnlyCollection<Artwork>> GetByCreatorIdAsync(int creatorId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Artwork>()
            .AsNoTracking()
            .Include(a => a.Creator)
            .Include(a => a.ArtworkTags)
                .ThenInclude(at => at.Tag)
            .Where(a => a.CreatorId == creatorId && !a.IsSoftDeleted)
            .OrderByDescending(a => a.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Artwork>> GetFeedAsync(int? cursor, int limit, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<Artwork>()
            .AsNoTracking()
            .Include(a => a.Creator)
            .Include(a => a.ArtworkTags)
                .ThenInclude(at => at.Tag)
            .Where(a => !a.IsSoftDeleted);

        if (cursor.HasValue)
        {
            query = query.Where(a => a.Id < cursor.Value);
        }

        return await query
            .OrderByDescending(a => a.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Artwork>> SearchAsync(string query, int skip, int take, CancellationToken cancellationToken = default)
    {
        var searchTerm = $"%{query}%";
        return await _dbContext.Set<Artwork>()
            .AsNoTracking()
            .Include(a => a.Creator)
            .Include(a => a.ArtworkTags)
                .ThenInclude(at => at.Tag)
            .Where(a => !a.IsSoftDeleted && (EF.Functions.ILike(a.Title, searchTerm) || EF.Functions.ILike(a.Description, searchTerm)))
            .OrderByDescending(a => a.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Artwork>> GetTrendingAsync(int limit, CancellationToken cancellationToken = default)
    {
        var weekAgo = DateTime.UtcNow.AddDays(-7);
        return await _dbContext.Set<Artwork>()
            .AsNoTracking()
            .Include(a => a.Creator)
            .Include(a => a.ArtworkTags)
                .ThenInclude(at => at.Tag)
            .Where(a => !a.IsSoftDeleted && a.CreatedAtUtc >= weekAgo)
            .OrderByDescending(a => a.LikeCount)
            .ThenByDescending(a => a.ViewCount)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Artwork>> GetByTagAsync(string tagSlug, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Artwork>()
            .AsNoTracking()
            .Include(a => a.Creator)
            .Include(a => a.ArtworkTags)
                .ThenInclude(at => at.Tag)
            .Where(a => !a.IsSoftDeleted && a.ArtworkTags.Any(at => at.Tag != null && at.Tag.Slug == tagSlug))
            .OrderByDescending(a => a.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Artwork>> GetRecommendedAsync(int userId, int limit, CancellationToken cancellationToken = default)
    {
        // Get tags of artworks the user has liked or bookmarked
        var favoriteTags = await _dbContext.Likes
            .Where(l => l.UserId == userId)
            .SelectMany(l => l.Artwork!.ArtworkTags.Select(at => at.TagId))
            .Union(
                _dbContext.Bookmarks
                .Where(b => b.UserId == userId)
                .SelectMany(b => b.Artwork!.ArtworkTags.Select(at => at.TagId))
            )
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!favoriteTags.Any())
        {
            // Fallback to trending if no interactions
            return await GetTrendingAsync(limit, cancellationToken);
        }

        // Find artworks with these tags that the user hasn't liked or bookmarked yet
        return await _dbContext.Set<Artwork>()
            .AsNoTracking()
            .Include(a => a.Creator)
                .ThenInclude(u => u!.UserProfile)
            .Include(a => a.ArtworkTags)
                .ThenInclude(at => at.Tag)
            .Where(a => a.IsApproved && !a.IsSoftDeleted && a.CreatorId != userId) // Exclude own artworks
            .Where(a => !_dbContext.Likes.Any(l => l.UserId == userId && l.ArtworkId == a.Id)) // Exclude liked
            .Where(a => !_dbContext.Bookmarks.Any(b => b.UserId == userId && b.ArtworkId == a.Id)) // Exclude bookmarked
            .Where(a => a.ArtworkTags.Any(at => favoriteTags.Contains(at.TagId)))
            .OrderByDescending(a => a.ArtworkTags.Count(at => favoriteTags.Contains(at.TagId))) // Sort by tag match score
            .ThenByDescending(a => a.LikeCount + a.ViewCount) // Tie breaker: popularity
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Artwork>> GetLikedByUserIdAsync(int userId, int skip, int take, CancellationToken cancellationToken = default)
    {
        var artworkIds = await _dbContext.Likes
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .Select(l => l.ArtworkId)
            .ToListAsync(cancellationToken);

        if (!artworkIds.Any()) return new List<Artwork>();

        var artworks = await _dbContext.Set<Artwork>()
            .AsNoTracking()
            .Include(a => a.Creator)
            .Include(a => a.ArtworkTags)
                .ThenInclude(at => at.Tag)
            .Where(a => artworkIds.Contains(a.Id) && !a.IsSoftDeleted)
            .ToListAsync(cancellationToken);

        return artworkIds.Select(id => artworks.FirstOrDefault(a => a.Id == id)).Where(a => a != null).Select(a => a!).ToList();
    }

    public async Task<IReadOnlyCollection<Artwork>> GetBookmarkedByUserIdAsync(int userId, int skip, int take, CancellationToken cancellationToken = default)
    {
        var artworkIds = await _dbContext.Bookmarks
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .Select(b => b.ArtworkId)
            .ToListAsync(cancellationToken);

        if (!artworkIds.Any()) return new List<Artwork>();

        var artworks = await _dbContext.Set<Artwork>()
            .AsNoTracking()
            .Include(a => a.Creator)
            .Include(a => a.ArtworkTags)
                .ThenInclude(at => at.Tag)
            .Where(a => artworkIds.Contains(a.Id) && !a.IsSoftDeleted)
            .ToListAsync(cancellationToken);

        return artworkIds.Select(id => artworks.FirstOrDefault(a => a.Id == id)).Where(a => a != null).Select(a => a!).ToList();
    }

    public async Task IncrementViewCountAsync(int artworkId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Artwork>()
            .Where(a => a.Id == artworkId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.ViewCount, a => a.ViewCount + 1), cancellationToken);
    }

    public async Task IncrementLikeCountAsync(int artworkId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Artwork>()
            .Where(a => a.Id == artworkId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.LikeCount, a => a.LikeCount + 1), cancellationToken);
    }

    public async Task DecrementLikeCountAsync(int artworkId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Artwork>()
            .Where(a => a.Id == artworkId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.LikeCount, a => a.LikeCount > 0 ? a.LikeCount - 1 : 0), cancellationToken);
    }

    public async Task IncrementCommentCountAsync(int artworkId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Artwork>()
            .Where(a => a.Id == artworkId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.CommentCount, a => a.CommentCount + 1), cancellationToken);
    }

    public async Task DecrementCommentCountAsync(int artworkId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Artwork>()
            .Where(a => a.Id == artworkId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.CommentCount, a => a.CommentCount > 0 ? a.CommentCount - 1 : 0), cancellationToken);
    }
}
