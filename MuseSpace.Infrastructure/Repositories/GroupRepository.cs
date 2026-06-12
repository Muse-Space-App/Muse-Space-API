using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    private readonly MuseSpaceDbContext _context;

    public GroupRepository(MuseSpaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsUserInGroupAsync(int groupId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId, cancellationToken);
    }

    public async Task<GroupMember?> GetGroupMemberAsync(int groupId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId, cancellationToken);
    }

    public async Task AddGroupMemberAsync(GroupMember member, CancellationToken cancellationToken = default)
    {
        await _context.GroupMembers.AddAsync(member, cancellationToken);
        await _context.Groups
            .Where(g => g.Id == member.GroupId)
            .ExecuteUpdateAsync(s => s.SetProperty(g => g.MemberCount, g => g.MemberCount + 1), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveGroupMemberAsync(GroupMember member, CancellationToken cancellationToken = default)
    {
        _context.GroupMembers.Remove(member);
        await _context.Groups
            .Where(g => g.Id == member.GroupId && g.MemberCount > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(g => g.MemberCount, g => g.MemberCount - 1), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateGroupMemberAsync(GroupMember member, CancellationToken cancellationToken = default)
    {
        _context.GroupMembers.Update(member);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Group>> GetUserGroupsAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .Where(gm => gm.UserId == userId)
            .Include(gm => gm.Group)
            .OrderByDescending(gm => gm.JoinedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(gm => gm.Group!)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<GroupMember>> GetGroupMembersAsync(int groupId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .Where(gm => gm.GroupId == groupId)
            .Include(gm => gm.User)
                .ThenInclude(u => u!.UserProfile)
            .OrderByDescending(gm => gm.JoinedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<GroupPost>> GetGroupPostsAsync(int groupId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.GroupPosts
            .Where(gp => gp.GroupId == groupId)
            .Include(gp => gp.Author)
                .ThenInclude(a => a!.UserProfile)
            .OrderByDescending(gp => gp.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddGroupPostAsync(GroupPost post, CancellationToken cancellationToken = default)
    {
        await _context.GroupPosts.AddAsync(post, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<GroupPost?> GetGroupPostByIdAsync(int postId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupPosts
            .Include(gp => gp.Author)
            .FirstOrDefaultAsync(gp => gp.Id == postId, cancellationToken);
    }

    public async Task UpdateGroupPostAsync(GroupPost post, CancellationToken cancellationToken = default)
    {
        _context.GroupPosts.Update(post);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteGroupPostAsync(GroupPost post, CancellationToken cancellationToken = default)
    {
        _context.GroupPosts.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
