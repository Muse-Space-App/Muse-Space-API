using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IReadOnlyCollection<Comment>> GetByArtworkIdAsync(int artworkId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByArtworkIdAsync(int artworkId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Comment>> GetRepliesAsync(int commentId, int page, int pageSize, CancellationToken cancellationToken = default);
}
