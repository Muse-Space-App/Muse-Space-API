using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface IGroupRepository : IRepository<Group>
{
    Task<bool> IsUserInGroupAsync(int groupId, int userId, CancellationToken cancellationToken = default);
    Task<GroupMember?> GetGroupMemberAsync(int groupId, int userId, CancellationToken cancellationToken = default);
    Task AddGroupMemberAsync(GroupMember member, CancellationToken cancellationToken = default);
    Task RemoveGroupMemberAsync(GroupMember member, CancellationToken cancellationToken = default);
    Task UpdateGroupMemberAsync(GroupMember member, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Group>> GetUserGroupsAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<GroupMember>> GetGroupMembersAsync(int groupId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GroupPost>> GetGroupPostsAsync(int groupId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddGroupPostAsync(GroupPost post, CancellationToken cancellationToken = default);
    Task<GroupPost?> GetGroupPostByIdAsync(int postId, CancellationToken cancellationToken = default);
    Task UpdateGroupPostAsync(GroupPost post, CancellationToken cancellationToken = default);
    Task DeleteGroupPostAsync(GroupPost post, CancellationToken cancellationToken = default);
}
