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

    public override async Task<Group?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.Creator)
                .ThenInclude(u => u!.UserProfile)
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyCollection<Group>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.Creator)
                .ThenInclude(u => u!.UserProfile)
            .Include(g => g.Members)
            .ToListAsync(cancellationToken);
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
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveGroupMemberAsync(GroupMember member, CancellationToken cancellationToken = default)
    {
        _context.GroupMembers.Remove(member);
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
                .ThenInclude(g => g!.Members)
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
            .Include(gp => gp.Likes)
            .Include(gp => gp.Comments.Where(c => !c.IsSoftDeleted))
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

    public async Task<GroupPostLike?> GetPostLikeAsync(int userId, int postId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupPostLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.GroupPostId == postId, cancellationToken);
    }

    public async Task AddPostLikeAsync(GroupPostLike like, CancellationToken cancellationToken = default)
    {
        await _context.GroupPostLikes.AddAsync(like, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemovePostLikeAsync(GroupPostLike like, CancellationToken cancellationToken = default)
    {
        _context.GroupPostLikes.Remove(like);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetPostLikeCountAsync(int postId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupPostLikes
            .CountAsync(l => l.GroupPostId == postId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<GroupPostComment>> GetPostCommentsAsync(int postId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.GroupPostComments
            .Where(c => c.GroupPostId == postId && !c.IsSoftDeleted)
            .Include(c => c.User)
                .ThenInclude(u => u!.UserProfile)
            .OrderBy(c => c.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddPostCommentAsync(GroupPostComment comment, CancellationToken cancellationToken = default)
    {
        await _context.GroupPostComments.AddAsync(comment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<GroupPostComment?> GetPostCommentByIdAsync(int commentId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupPostComments
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsSoftDeleted, cancellationToken);
    }

    public async Task DeletePostCommentAsync(GroupPostComment comment, CancellationToken cancellationToken = default)
    {
        comment.IsSoftDeleted = true;
        _context.GroupPostComments.Update(comment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
