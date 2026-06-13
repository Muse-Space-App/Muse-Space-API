using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface ISearchService
{
    Task<GenericResult<SearchResponse>> SearchAsync(string? query, string? type, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<ArtworkResponse>>> AdvancedSearchAsync(AdvancedSearchRequest request, CancellationToken cancellationToken = default);
}

public interface IDashboardService
{
    Task<GenericResult<DashboardStatsResponse>> GetCreatorDashboardStatsAsync(int creatorId, CancellationToken cancellationToken = default);
}
