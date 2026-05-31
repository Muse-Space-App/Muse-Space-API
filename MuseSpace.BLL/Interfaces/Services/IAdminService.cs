using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface IAdminService
{
    Task<GenericResult<AdminStatsResponse>> GetAdminStatsAsync(CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<ReportResponse>>> GetPendingReportsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<ReportResponse>> ReviewReportAsync(int reportId, int adminId, ReviewReportRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> BanUserAsync(BanUserRequest request, int adminId, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> UnbanUserAsync(int userId, int adminId, CancellationToken cancellationToken = default);
}
