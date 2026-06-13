using Microsoft.EntityFrameworkCore;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.BLL.Services;

public class ReportService : IReportService
{
    private readonly MuseSpaceDbContext _dbContext;

    public ReportService(MuseSpaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenericResult<bool>> CreateReportAsync(int reportedById, CreateReportRequest request, CancellationToken cancellationToken = default)
    {
        if (request.ArtworkId == null && request.TargetUserId == null)
        {
            return GenericResult<bool>.Failure("Must specify either ArtworkId or TargetUserId", ErrorType.ValidationFailed);
        }

        var report = new Report
        {
            ReportedById = reportedById,
            ArtworkId = request.ArtworkId,
            TargetUserId = request.TargetUserId,
            ReportType = request.ReportType,
            Reason = request.Reason,
            Status = "Pending",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Reports.Add(report);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return GenericResult<bool>.Success(true);
    }
}
