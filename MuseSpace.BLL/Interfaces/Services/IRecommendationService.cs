using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface IRecommendationService
{
    Task<GenericResult<PagedResult<ArtworkResponse>>> GetRecommendedArtworksAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<ArtworkResponse>>> GetSimilarArtworksAsync(int artworkId, int page, int pageSize, CancellationToken cancellationToken = default);
}
