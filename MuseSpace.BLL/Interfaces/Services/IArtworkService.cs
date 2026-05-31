using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Response;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface IArtworkService
{
    Task<GenericResult<ArtworkResponse>> CreateAsync(int userId, CreateArtworkRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<ArtworkResponse>> GetByIdAsync(int id, int? currentUserId = null, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<ArtworkResponse>>> GetByCreatorIdAsync(int creatorId, int page, int pageSize, int? currentUserId = null, CancellationToken cancellationToken = default);
    Task<GenericResult<ArtworkResponse>> UpdateAsync(int id, int userId, UpdateArtworkRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<ArtworkResponse>>> SearchAsync(string query, int page, int pageSize, int? currentUserId = null, CancellationToken cancellationToken = default);
    Task<GenericResult<ArtworkFeedResponse>> GetFeedAsync(int? cursor, int limit, int? currentUserId = null, CancellationToken cancellationToken = default);
    Task<GenericResult<IReadOnlyCollection<ArtworkResponse>>> GetTrendingAsync(int limit, int? currentUserId = null, CancellationToken cancellationToken = default);
}
