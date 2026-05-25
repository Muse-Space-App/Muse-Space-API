using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public class CommissionRepository : Repository<Commission>, ICommissionRepository
{
    private readonly MuseSpaceDbContext _context;

    public CommissionRepository(MuseSpaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Commission>> GetCommissionsByRequesterAsync(int requesterId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.Commissions
            .AsNoTracking()
            .Include(c => c.Artist)
                .ThenInclude(u => u!.UserProfile)
            .Where(c => c.RequesterId == requesterId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Commission>> GetCommissionsByArtistAsync(int artistId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.Commissions
            .AsNoTracking()
            .Include(c => c.Requester)
                .ThenInclude(u => u!.UserProfile)
            .Where(c => c.ArtistId == artistId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<Commission?> GetCommissionWithDetailsAsync(int commissionId, CancellationToken cancellationToken = default)
    {
        return await _context.Commissions
            .Include(c => c.Requester)
                .ThenInclude(u => u!.UserProfile)
            .Include(c => c.Artist)
                .ThenInclude(u => u!.UserProfile)
            .FirstOrDefaultAsync(c => c.Id == commissionId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CommissionMessage>> GetCommissionMessagesAsync(int commissionId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.CommissionMessages
            .AsNoTracking()
            .Include(m => m.Sender)
                .ThenInclude(u => u!.UserProfile)
            .Where(m => m.CommissionId == commissionId)
            .OrderByDescending(m => m.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task AddMessageAsync(CommissionMessage message, CancellationToken cancellationToken = default)
    {
        await _context.CommissionMessages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkMessagesAsReadAsync(int commissionId, int userId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE CommissionMessages SET IsRead = true WHERE CommissionId = {0} AND SenderId != {1} AND IsRead = false",
            new object[] { commissionId, userId },
            cancellationToken);
    }
}
