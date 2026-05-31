using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public class SocialRepository : ISocialRepository
{
    private readonly MuseSpaceDbContext _context;

    public SocialRepository(MuseSpaceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsFollowingAsync(int followerId, int followingId, CancellationToken cancellationToken = default)
    {
        return await _context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, cancellationToken);
    }

    public async Task AddFollowAsync(Follow follow, CancellationToken cancellationToken = default)
    {
        await _context.Follows.AddAsync(follow, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFollowAsync(Follow follow, CancellationToken cancellationToken = default)
    {
        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetFollowersAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Follows
            .Where(f => f.FollowingId == userId)
            .Include(f => f.Follower)
                .ThenInclude(u => u!.UserProfile)
            .OrderByDescending(f => f.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => f.Follower!)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetFollowingAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Follows
            .Where(f => f.FollowerId == userId)
            .Include(f => f.FollowingUser)
                .ThenInclude(u => u!.UserProfile)
            .OrderByDescending(f => f.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => f.FollowingUser!)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetFollowerCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Follows.CountAsync(f => f.FollowingId == userId, cancellationToken);
    }

    public async Task<int> GetFollowingCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Follows.CountAsync(f => f.FollowerId == userId, cancellationToken);
    }
}
