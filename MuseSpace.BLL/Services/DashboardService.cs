using Microsoft.EntityFrameworkCore;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.BLL.Services;

public class DashboardService : IDashboardService
{
    private readonly MuseSpaceDbContext _context;

    public DashboardService(MuseSpaceDbContext context)
    {
        _context = context;
    }

    public async Task<GenericResult<DashboardStatsResponse>> GetCreatorDashboardStatsAsync(int creatorId, CancellationToken cancellationToken = default)
    {
        // Get Artworks aggregates
        var artworksInfo = await _context.Artwork
            .Where(a => a.CreatorId == creatorId && !a.IsSoftDeleted)
            .GroupBy(a => a.CreatorId)
            .Select(g => new
            {
                TotalViews = g.Sum(a => a.ViewCount),
                TotalLikes = g.Sum(a => a.LikeCount),
                TotalComments = g.Sum(a => a.CommentCount)
            })
            .FirstOrDefaultAsync(cancellationToken);

        // Get Followers count
        var totalFollowers = await _context.Follows
            .CountAsync(f => f.FollowingId == creatorId, cancellationToken);

        // Get Commissions aggregates
        var commissionsInfo = await _context.Commissions
            .Where(c => c.ArtistId == creatorId)
            .GroupBy(c => c.ArtistId)
            .Select(g => new
            {
                PendingCommissions = g.Count(c => c.Status == CommissionStatus.Pending),
                ActiveCommissions = g.Count(c => c.Status == CommissionStatus.InProgress || c.Status == CommissionStatus.Accepted),
                CompletedCommissions = g.Count(c => c.Status == CommissionStatus.Completed),
                TotalRevenue = g.Where(c => c.Status == CommissionStatus.Completed).Sum(c => c.Price)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var stats = new DashboardStatsResponse
        {
            TotalViews = artworksInfo?.TotalViews ?? 0,
            TotalLikes = artworksInfo?.TotalLikes ?? 0,
            TotalComments = artworksInfo?.TotalComments ?? 0,
            TotalFollowers = totalFollowers,
            PendingCommissions = commissionsInfo?.PendingCommissions ?? 0,
            ActiveCommissions = commissionsInfo?.ActiveCommissions ?? 0,
            CompletedCommissions = commissionsInfo?.CompletedCommissions ?? 0,
            TotalRevenue = commissionsInfo?.TotalRevenue ?? 0
        };

        return GenericResult<DashboardStatsResponse>.Success(stats);
    }
}
