using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface IGroupService
{
    Task<GenericResult<GroupResponse>> CreateGroupAsync(int userId, CreateGroupRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<GroupResponse>> UpdateGroupAsync(int groupId, int userId, UpdateGroupRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<GroupResponse>> GetGroupAsync(int groupId, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<GroupResponse>>> GetUserGroupsAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<GroupResponse>>> GetAllGroupsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<GenericResult<bool>> JoinGroupAsync(int groupId, int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> LeaveGroupAsync(int groupId, int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<GroupMemberResponse>>> GetGroupMembersAsync(int groupId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<GenericResult<GroupPostResponse>> CreateGroupPostAsync(int groupId, int userId, CreateGroupPostRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<GroupPostResponse>>> GetGroupPostsAsync(int groupId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> DeleteGroupPostAsync(int postId, int userId, CancellationToken cancellationToken = default);
}
