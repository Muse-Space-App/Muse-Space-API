using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface IInteractionService
{
    // Likes
    Task<GenericResult<bool>> ToggleLikeAsync(int artworkId, int userId, CancellationToken cancellationToken = default);

    // Bookmarks
    Task<GenericResult<bool>> ToggleBookmarkAsync(int artworkId, int userId, CancellationToken cancellationToken = default);

    // Shares
    Task<GenericResult<bool>> RecordShareAsync(int artworkId, int userId, string platform, CancellationToken cancellationToken = default);
}
