using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface IInteractionRepository
{
    // Likes
    Task<bool> HasUserLikedAsync(int artworkId, int userId, CancellationToken cancellationToken = default);
    Task AddLikeAsync(Like like, CancellationToken cancellationToken = default);
    Task RemoveLikeAsync(Like like, CancellationToken cancellationToken = default);

    // Bookmarks
    Task<bool> HasUserBookmarkedAsync(int artworkId, int userId, CancellationToken cancellationToken = default);
    Task AddBookmarkAsync(Bookmark bookmark, CancellationToken cancellationToken = default);
    Task RemoveBookmarkAsync(Bookmark bookmark, CancellationToken cancellationToken = default);

    // Shares
    Task AddShareAsync(Share share, CancellationToken cancellationToken = default);

    Task<Dictionary<int, bool>> GetUserLikesForArtworksAsync(IEnumerable<int> artworkIds, int userId, CancellationToken cancellationToken = default);
    Task<Dictionary<int, bool>> GetUserBookmarksForArtworksAsync(IEnumerable<int> artworkIds, int userId, CancellationToken cancellationToken = default);
}
