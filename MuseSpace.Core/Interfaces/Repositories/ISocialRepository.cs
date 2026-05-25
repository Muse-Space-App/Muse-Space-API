using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface ISocialRepository
{
    Task<bool> IsFollowingAsync(int followerId, int followingId, CancellationToken cancellationToken = default);
    Task AddFollowAsync(Follow follow, CancellationToken cancellationToken = default);
    Task RemoveFollowAsync(Follow follow, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<User>> GetFollowersAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<User>> GetFollowingAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<int> GetFollowerCountAsync(int userId, CancellationToken cancellationToken = default);
    Task<int> GetFollowingCountAsync(int userId, CancellationToken cancellationToken = default);
}
