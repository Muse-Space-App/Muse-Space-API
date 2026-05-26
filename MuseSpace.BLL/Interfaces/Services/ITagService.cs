using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface ITagService
{
    Task<GenericResult<IReadOnlyCollection<TagResponse>>> GetPopularAsync(int limit, CancellationToken cancellationToken = default);
    Task<GenericResult<IReadOnlyCollection<TagResponse>>> GetByArtworkIdAsync(int artworkId, CancellationToken cancellationToken = default);
}
