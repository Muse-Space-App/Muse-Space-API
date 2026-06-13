using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface IReportService
{
    Task<GenericResult<bool>> CreateReportAsync(int reportedById, CreateReportRequest request, CancellationToken cancellationToken = default);
}
