using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface ISocialService
{
    Task<GenericResult<bool>> ToggleFollowAsync(int followerId, int followingId, CancellationToken cancellationToken = default);
    Task<GenericResult<UserProfileResponse>> GetUserProfileAsync(int targetUserId, int? requestingUserId, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<FollowerResponse>>> GetFollowersAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<FollowerResponse>>> GetFollowingAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
}
