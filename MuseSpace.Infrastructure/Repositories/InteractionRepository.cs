using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public class InteractionRepository : IInteractionRepository
{
    private readonly MuseSpaceDbContext _context;

    public InteractionRepository(MuseSpaceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasUserLikedAsync(int artworkId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Likes.AnyAsync(l => l.ArtworkId == artworkId && l.UserId == userId, cancellationToken);
    }

    public async Task AddLikeAsync(Like like, CancellationToken cancellationToken = default)
    {
        await _context.Likes.AddAsync(like, cancellationToken);
        await _context.Artwork
            .Where(a => a.Id == like.ArtworkId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.LikeCount, a => a.LikeCount + 1), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveLikeAsync(Like like, CancellationToken cancellationToken = default)
    {
        _context.Likes.Remove(like);
        await _context.Artwork
            .Where(a => a.Id == like.ArtworkId && a.LikeCount > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.LikeCount, a => a.LikeCount - 1), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasUserBookmarkedAsync(int artworkId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookmarks.AnyAsync(b => b.ArtworkId == artworkId && b.UserId == userId, cancellationToken);
    }

    public async Task AddBookmarkAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        await _context.Bookmarks.AddAsync(bookmark, cancellationToken);
        await _context.Artwork
            .Where(a => a.Id == bookmark.ArtworkId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.BookmarkCount, a => a.BookmarkCount + 1), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveBookmarkAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        _context.Bookmarks.Remove(bookmark);
        await _context.Artwork
            .Where(a => a.Id == bookmark.ArtworkId && a.BookmarkCount > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.BookmarkCount, a => a.BookmarkCount - 1), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddShareAsync(Share share, CancellationToken cancellationToken = default)
    {
        await _context.Shares.AddAsync(share, cancellationToken);
        await _context.Artwork
            .Where(a => a.Id == share.ArtworkId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.ShareCount, a => a.ShareCount + 1), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Dictionary<int, bool>> GetUserLikesForArtworksAsync(IEnumerable<int> artworkIds, int userId, CancellationToken cancellationToken = default)
    {
        var likes = await _context.Likes
            .Where(l => l.UserId == userId && artworkIds.Contains(l.ArtworkId))
            .Select(l => l.ArtworkId)
            .ToListAsync(cancellationToken);

        return artworkIds.ToDictionary(id => id, id => likes.Contains(id));
    }

    public async Task<Dictionary<int, bool>> GetUserBookmarksForArtworksAsync(IEnumerable<int> artworkIds, int userId, CancellationToken cancellationToken = default)
    {
        var bookmarks = await _context.Bookmarks
            .Where(b => b.UserId == userId && artworkIds.Contains(b.ArtworkId))
            .Select(b => b.ArtworkId)
            .ToListAsync(cancellationToken);

        return artworkIds.ToDictionary(id => id, id => bookmarks.Contains(id));
    }
}
