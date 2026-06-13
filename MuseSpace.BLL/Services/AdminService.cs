using Microsoft.EntityFrameworkCore;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.BLL.Services;

public class AdminService : IAdminService
{
    private readonly MuseSpaceDbContext _dbContext;

    public AdminService(MuseSpaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenericResult<AdminStatsResponse>> GetAdminStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalUsers = await _dbContext.Users.CountAsync(cancellationToken);
        var totalArtworks = await _dbContext.Set<MuseSpace.Core.Entities.Artwork>().CountAsync(cancellationToken);
        var totalReports = await _dbContext.Reports.CountAsync(cancellationToken);
        var pendingReports = await _dbContext.Reports.CountAsync(r => r.Status == "Pending", cancellationToken);
        var bannedUsers = await _dbContext.Users.CountAsync(u => u.IsBanned, cancellationToken);

        return GenericResult<AdminStatsResponse>.Success(new AdminStatsResponse
        {
            TotalUsers = totalUsers,
            TotalArtworks = totalArtworks,
            TotalReports = totalReports,
            PendingReports = pendingReports,
            BannedUsers = bannedUsers
        });
    }

    public async Task<GenericResult<PagedResult<ReportResponse>>> GetPendingReportsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Reports
            .Include(r => r.ReportedBy)
            .Include(r => r.Artwork)
            .Include(r => r.TargetUser)
            .Where(r => r.Status == "Pending")
            .AsQueryable();

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReportResponse
            {
                Id = r.Id,
                ArtworkId = r.ArtworkId,
                ArtworkTitle = r.Artwork != null ? r.Artwork.Title : "",
                TargetUserId = r.TargetUserId,
                TargetUsername = r.TargetUser != null ? r.TargetUser.Username : "",
                ReportedById = r.ReportedById,
                ReportedByUsername = r.ReportedBy!.Username,
                ReportType = r.ReportType,
                Reason = r.Reason,
                Status = r.Status,
                CreatedAtUtc = r.CreatedAtUtc,
                ReviewedAtUtc = r.ReviewedAtUtc,
                AdminNotes = r.AdminNotes
            })
            .ToListAsync(cancellationToken);

        return GenericResult<PagedResult<ReportResponse>>.Success(new PagedResult<ReportResponse>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalItems
        });
    }

    public async Task<GenericResult<ReportResponse>> ReviewReportAsync(int reportId, int adminId, ReviewReportRequest request, CancellationToken cancellationToken = default)
    {
        var report = await _dbContext.Reports
            .Include(r => r.Artwork)
            .Include(r => r.ReportedBy)
            .Include(r => r.TargetUser)
            .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken);

        if (report == null)
            return GenericResult<ReportResponse>.Failure("Report not found", ErrorType.NotFound);

        report.Status = request.Status;
        report.AdminNotes = request.AdminNotes;
        report.ReviewedAtUtc = DateTime.UtcNow;
        report.ReviewedByAdminId = adminId;
        report.UpdatedAtUtc = DateTime.UtcNow;

        if (request.Status == "Approved")
        {
            if (report.ArtworkId.HasValue && report.Artwork != null)
            {
                report.Artwork.IsSoftDeleted = true;
            }
            else if (report.TargetUserId.HasValue)
            {
                var targetUser = await _dbContext.Users.FindAsync(new object[] { report.TargetUserId.Value }, cancellationToken);
                if (targetUser != null)
                {
                    targetUser.IsBanned = true;
                }
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return GenericResult<ReportResponse>.Success(new ReportResponse
        {
            Id = report.Id,
            ArtworkId = report.ArtworkId,
            ArtworkTitle = report.Artwork?.Title ?? "",
            TargetUserId = report.TargetUserId,
            TargetUsername = report.TargetUser?.Username ?? "",
            ReportedById = report.ReportedById,
            ReportedByUsername = report.ReportedBy?.Username ?? "",
            ReportType = report.ReportType,
            Reason = report.Reason,
            Status = report.Status,
            CreatedAtUtc = report.CreatedAtUtc,
            ReviewedAtUtc = report.ReviewedAtUtc,
            AdminNotes = report.AdminNotes
        });
    }

    public async Task<GenericResult<bool>> BanUserAsync(BanUserRequest request, int adminId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            return GenericResult<bool>.Failure("User not found", ErrorType.NotFound);

        user.IsBanned = true;
        user.BanReason = request.Reason;
        if (request.BanDurationDays.HasValue)
        {
            user.BanExpiryUtc = DateTime.UtcNow.AddDays(request.BanDurationDays.Value);
        }
        user.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return GenericResult<bool>.Success(true);
    }

    public async Task<GenericResult<bool>> UnbanUserAsync(int userId, int adminId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
            return GenericResult<bool>.Failure("User not found", ErrorType.NotFound);

        user.IsBanned = false;
        user.BanReason = null;
        user.BanExpiryUtc = null;
        user.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return GenericResult<bool>.Success(true);
    }
}
