using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Tag>> GetByArtworkIdAsync(int artworkId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Tag>> GetPopularAsync(int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Tag>> SearchAsync(string query, int skip, int take, CancellationToken cancellationToken = default);

    Task<Tag> GetOrCreateAsync(string name, string createdBy, CancellationToken cancellationToken = default);
    Task IncrementUsageCountAsync(int tagId, CancellationToken cancellationToken = default);
}
