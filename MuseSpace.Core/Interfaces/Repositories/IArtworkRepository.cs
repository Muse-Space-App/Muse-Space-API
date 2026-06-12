using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface IArtworkRepository : IRepository<Artwork>
{
    Task<Artwork?> GetByIdWithTagsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Artwork>> GetByCreatorIdAsync(int creatorId, int skip, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Artwork>> GetFeedAsync(int? cursor, int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Artwork>> SearchAsync(string query, int skip, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Artwork>> GetTrendingAsync(int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Artwork>> GetByTagAsync(string tagSlug, int skip, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Artwork>> GetRecommendedAsync(int userId, int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Artwork>> GetLikedByUserIdAsync(int userId, int skip, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Artwork>> GetBookmarkedByUserIdAsync(int userId, int skip, int take, CancellationToken cancellationToken = default);

    Task IncrementViewCountAsync(int artworkId, CancellationToken cancellationToken = default);
    Task IncrementLikeCountAsync(int artworkId, CancellationToken cancellationToken = default);
    Task DecrementLikeCountAsync(int artworkId, CancellationToken cancellationToken = default);
    Task IncrementCommentCountAsync(int artworkId, CancellationToken cancellationToken = default);
    Task DecrementCommentCountAsync(int artworkId, CancellationToken cancellationToken = default);
}
