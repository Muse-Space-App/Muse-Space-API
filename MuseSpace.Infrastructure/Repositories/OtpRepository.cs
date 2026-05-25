using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public sealed class OtpRepository : IOtpRepository
{
    private readonly MuseSpaceDbContext _context;

    public OtpRepository(MuseSpaceDbContext context)
    {
        _context = context;
    }

    public async Task<Otp?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Otps.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyCollection<Otp>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return (await _context.Otps.ToListAsync(cancellationToken)).AsReadOnly();
    }

    public async Task AddAsync(Otp entity, CancellationToken cancellationToken = default)
    {
        await _context.Otps.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Otp entity, CancellationToken cancellationToken = default)
    {
        _context.Otps.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var otp = await GetByIdAsync(id, cancellationToken);
        if (otp != null)
        {
            _context.Otps.Remove(otp);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AddRangeAsync(IEnumerable<Otp> entities, CancellationToken cancellationToken = default)
    {
        await _context.Otps.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<Otp> entities, CancellationToken cancellationToken = default)
    {
        _context.Otps.UpdateRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Update(Otp entity)
    {
        _context.Otps.Update(entity);
        _context.SaveChanges();
    }

    public void Delete(Otp entity)
    {
        _context.Otps.Remove(entity);
        _context.SaveChanges();
    }

    public async Task<Otp?> GetLatestValidOtpAsync(
        int userId,
        string purpose,
        CancellationToken cancellationToken = default)
    {
        return await _context.Otps
            .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task InvalidateUserOtpsAsync(
        int userId,
        string purpose,
        CancellationToken cancellationToken = default)
    {
        var otps = await _context.Otps
            .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (var otp in otps)
        {
            otp.IsUsed = true;
        }

        if (otps.Any())
        {
            _context.Otps.UpdateRange(otps);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
