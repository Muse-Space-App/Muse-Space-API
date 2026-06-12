using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    private readonly MuseSpaceDbContext _context;

    public CommentRepository(MuseSpaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<int> CountByArtworkIdAsync(int artworkId, CancellationToken cancellationToken = default)
    {
        return await _context.Comments
            .Where(c => c.ArtworkId == artworkId && !c.IsSoftDeleted && c.ParentCommentId == null)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Comment>> GetByArtworkIdAsync(int artworkId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Comments
            .Include(c => c.User)
                .ThenInclude(u => u!.UserProfile)
            .Where(c => c.ArtworkId == artworkId && !c.IsSoftDeleted && c.ParentCommentId == null)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Comment>> GetRepliesAsync(int commentId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Comments
            .Include(c => c.User)
                .ThenInclude(u => u!.UserProfile)
            .Where(c => c.ParentCommentId == commentId && !c.IsSoftDeleted)
            .OrderBy(c => c.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public override async Task AddAsync(Comment entity, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(entity, cancellationToken);

        // Update Artwork CommentCount
        await _context.Artwork
            .Where(a => a.Id == entity.ArtworkId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.CommentCount, a => a.CommentCount + 1), cancellationToken);
    }
}
