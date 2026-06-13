using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IMapper _mapper;

    public GroupService(IGroupRepository groupRepository, IMapper mapper)
    {
        _groupRepository = groupRepository;
        _mapper = mapper;
    }

    public async Task<GenericResult<GroupResponse>> CreateGroupAsync(int userId, CreateGroupRequest request, CancellationToken cancellationToken = default)
    {
        var group = new Group
        {
            Name = request.Name,
            Description = request.Description,
            CreatorId = userId,
            IsPrivate = request.IsPrivate,
            MemberCount = 0
        };

        await _groupRepository.AddAsync(group, cancellationToken);

        // Add creator as Admin member
        var member = new GroupMember
        {
            GroupId = group.Id,
            UserId = userId,
            Role = "Admin",
            JoinedAtUtc = DateTime.UtcNow
        };
        await _groupRepository.AddGroupMemberAsync(member, cancellationToken);

        // Re-fetch to get accurate MemberCount from actual GroupMembers rows
        var updatedGroup = await _groupRepository.GetByIdAsync(group.Id, cancellationToken) ?? group;
        var response = _mapper.Map<GroupResponse>(updatedGroup);
        return GenericResult<GroupResponse>.Success(response, "Group created successfully");
    }

    public async Task<GenericResult<GroupResponse>> UpdateGroupAsync(int groupId, int userId, UpdateGroupRequest request, CancellationToken cancellationToken = default)
    {
        var group = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        if (group == null)
        {
            return GenericResult<GroupResponse>.Failure("Group not found", ErrorType.NotFound);
        }

        var member = await _groupRepository.GetGroupMemberAsync(groupId, userId, cancellationToken);
        if (member == null || member.Role != "Admin")
        {
            return GenericResult<GroupResponse>.Failure("Only admins can update the group", ErrorType.Unauthorized);
        }

        group.Name = request.Name;
        group.Description = request.Description;
        group.IsPrivate = request.IsPrivate;

        await _groupRepository.UpdateAsync(group, cancellationToken);

        var response = _mapper.Map<GroupResponse>(group);
        return GenericResult<GroupResponse>.Success(response, "Group updated successfully");
    }

    public async Task<GenericResult<GroupResponse>> GetGroupAsync(int groupId, int? currentUserId = null, CancellationToken cancellationToken = default)
    {
        var group = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        if (group == null)
        {
            return GenericResult<GroupResponse>.Failure("Group not found", ErrorType.NotFound);
        }

        var response = _mapper.Map<GroupResponse>(group);
        if (currentUserId.HasValue)
        {
            response.IsMember = await _groupRepository.IsUserInGroupAsync(groupId, currentUserId.Value, cancellationToken);
        }
        return GenericResult<GroupResponse>.Success(response);
    }

    public async Task<GenericResult<PagedResult<GroupResponse>>> GetUserGroupsAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var groups = await _groupRepository.GetUserGroupsAsync(userId, page, pageSize, cancellationToken);

        var responses = _mapper.Map<IReadOnlyCollection<GroupResponse>>(groups);

        // Accurate pagination count would need CountUserGroupsAsync in repo
        var pagedResult = new PagedResult<GroupResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = groups.Count == pageSize ? page * pageSize + 1 : (page - 1) * pageSize + groups.Count
        };

        return GenericResult<PagedResult<GroupResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<PagedResult<GroupResponse>>> GetAllGroupsAsync(int page, int pageSize, int? currentUserId = null, CancellationToken cancellationToken = default)
    {
        var groups = await _groupRepository.GetAllAsync(cancellationToken);
        
        var pagedGroups = groups.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var responses = _mapper.Map<IReadOnlyCollection<GroupResponse>>(pagedGroups);

        if (currentUserId.HasValue)
        {
            foreach (var response in responses)
            {
                response.IsMember = await _groupRepository.IsUserInGroupAsync(response.Id, currentUserId.Value, cancellationToken);
            }
        }

        var pagedResult = new PagedResult<GroupResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = groups.Count
        };

        return GenericResult<PagedResult<GroupResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<bool>> JoinGroupAsync(int groupId, int userId, CancellationToken cancellationToken = default)
    {
        var group = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        if (group == null)
        {
            return GenericResult<bool>.Failure("Group not found", ErrorType.NotFound);
        }

        var isMember = await _groupRepository.IsUserInGroupAsync(groupId, userId, cancellationToken);
        if (isMember)
        {
            return GenericResult<bool>.Failure("Already a member of this group", ErrorType.ValidationFailed);
        }

        var member = new GroupMember
        {
            GroupId = groupId,
            UserId = userId,
            Role = "Member",
            JoinedAtUtc = DateTime.UtcNow
        };

        await _groupRepository.AddGroupMemberAsync(member, cancellationToken);
        return GenericResult<bool>.Success(true, "Successfully joined group");
    }

    public async Task<GenericResult<bool>> LeaveGroupAsync(int groupId, int userId, CancellationToken cancellationToken = default)
    {
        var member = await _groupRepository.GetGroupMemberAsync(groupId, userId, cancellationToken);
        if (member == null)
        {
            return GenericResult<bool>.Failure("You are not a member of this group", ErrorType.ValidationFailed);
        }

        if (member.Role == "Admin")
        {
            // Simplified: prevent admin leaving for now without assigning another
            return GenericResult<bool>.Failure("Group admin cannot leave directly. Assign another admin first.", ErrorType.ValidationFailed);
        }

        await _groupRepository.RemoveGroupMemberAsync(member, cancellationToken);
        return GenericResult<bool>.Success(true, "Successfully left group");
    }

    public async Task<GenericResult<PagedResult<GroupMemberResponse>>> GetGroupMembersAsync(int groupId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var members = await _groupRepository.GetGroupMembersAsync(groupId, page, pageSize, cancellationToken);

        var responses = members.Select(m => new GroupMemberResponse
        {
            GroupId = m.GroupId,
            UserId = m.UserId,
            Username = m.User?.Username ?? string.Empty,
            AvatarUrl = m.User?.UserProfile?.AvatarUrl ?? string.Empty,
            Role = m.Role,
            JoinedAtUtc = m.JoinedAtUtc
        }).ToList();

        var pagedResult = new PagedResult<GroupMemberResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = members.Count == pageSize ? page * pageSize + 1 : (page - 1) * pageSize + members.Count
        };

        return GenericResult<PagedResult<GroupMemberResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<GroupPostResponse>> CreateGroupPostAsync(int groupId, int userId, CreateGroupPostRequest request, CancellationToken cancellationToken = default)
    {
        var isMember = await _groupRepository.IsUserInGroupAsync(groupId, userId, cancellationToken);
        if (!isMember)
        {
            return GenericResult<GroupPostResponse>.Failure("Only members can post in this group", ErrorType.Unauthorized);
        }

        var post = new GroupPost
        {
            GroupId = groupId,
            AuthorId = userId,
            Content = request.Content,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _groupRepository.AddGroupPostAsync(post, cancellationToken);

        var response = _mapper.Map<GroupPostResponse>(post);
        return GenericResult<GroupPostResponse>.Success(response, "Post created successfully");
    }

    public async Task<GenericResult<PagedResult<GroupPostResponse>>> GetGroupPostsAsync(int groupId, int page, int pageSize, int? currentUserId = null, CancellationToken cancellationToken = default)
    {
        var posts = await _groupRepository.GetGroupPostsAsync(groupId, page, pageSize, cancellationToken);

        var responses = posts.Select(post => new GroupPostResponse
        {
            Id = post.Id,
            GroupId = post.GroupId,
            AuthorId = post.AuthorId,
            AuthorUsername = post.Author?.Username ?? string.Empty,
            AuthorAvatarUrl = post.Author?.UserProfile?.AvatarUrl ?? string.Empty,
            Content = post.Content,
            IsEdited = post.IsEdited,
            CreatedAtUtc = post.CreatedAtUtc,
            EditedAtUtc = post.EditedAtUtc,
            LikeCount = post.Likes.Count,
            CommentCount = post.Comments.Count,
            IsLiked = currentUserId.HasValue && post.Likes.Any(l => l.UserId == currentUserId.Value)
        }).ToList();

        var pagedResult = new PagedResult<GroupPostResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = posts.Count == pageSize ? page * pageSize + 1 : (page - 1) * pageSize + posts.Count
        };

        return GenericResult<PagedResult<GroupPostResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<bool>> DeleteGroupPostAsync(int postId, int userId, CancellationToken cancellationToken = default)
    {
        var post = await _groupRepository.GetGroupPostByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            return GenericResult<bool>.Failure("Post not found", ErrorType.NotFound);
        }

        var member = await _groupRepository.GetGroupMemberAsync(post.GroupId, userId, cancellationToken);
        if (post.AuthorId != userId && (member == null || (member.Role != "Admin" && member.Role != "Moderator")))
        {
            return GenericResult<bool>.Failure("You do not have permission to delete this post", ErrorType.Unauthorized);
        }

        await _groupRepository.DeleteGroupPostAsync(post, cancellationToken);
        return GenericResult<bool>.Success(true, "Post deleted successfully");
    }

    public async Task<GenericResult<bool>> TogglePostLikeAsync(int userId, int postId, CancellationToken cancellationToken = default)
    {
        var post = await _groupRepository.GetGroupPostByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            return GenericResult<bool>.Failure("Post not found", ErrorType.NotFound);
        }

        var like = await _groupRepository.GetPostLikeAsync(userId, postId, cancellationToken);
        if (like != null)
        {
            await _groupRepository.RemovePostLikeAsync(like, cancellationToken);
            return GenericResult<bool>.Success(false, "Post unliked");
        }
        else
        {
            var newLike = new GroupPostLike
            {
                UserId = userId,
                GroupPostId = postId,
                CreatedAtUtc = DateTime.UtcNow
            };
            await _groupRepository.AddPostLikeAsync(newLike, cancellationToken);
            return GenericResult<bool>.Success(true, "Post liked");
        }
    }

    public async Task<GenericResult<GroupPostCommentResponse>> AddPostCommentAsync(int userId, int postId, CreateGroupPostCommentRequest request, CancellationToken cancellationToken = default)
    {
        var post = await _groupRepository.GetGroupPostByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            return GenericResult<GroupPostCommentResponse>.Failure("Post not found", ErrorType.NotFound);
        }

        var isMember = await _groupRepository.IsUserInGroupAsync(post.GroupId, userId, cancellationToken);
        if (!isMember)
        {
            return GenericResult<GroupPostCommentResponse>.Failure("Only members can comment on group posts", ErrorType.Unauthorized);
        }

        var comment = new GroupPostComment
        {
            GroupPostId = postId,
            UserId = userId,
            Content = request.Content,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _groupRepository.AddPostCommentAsync(comment, cancellationToken);

        // Fetch back to get user info
        var savedComment = await _groupRepository.GetPostCommentByIdAsync(comment.Id, cancellationToken);
        
        var response = new GroupPostCommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAtUtc = comment.CreatedAtUtc
        };

        if (savedComment?.User != null)
        {
            response.AuthorUsername = savedComment.User.Username;
            response.AuthorAvatarUrl = savedComment.User.UserProfile?.AvatarUrl ?? string.Empty;
        }

        return GenericResult<GroupPostCommentResponse>.Success(response, "Comment added successfully");
    }

    public async Task<GenericResult<PagedResult<GroupPostCommentResponse>>> GetPostCommentsAsync(int postId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var post = await _groupRepository.GetGroupPostByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            return GenericResult<PagedResult<GroupPostCommentResponse>>.Failure("Post not found", ErrorType.NotFound);
        }

        var comments = await _groupRepository.GetPostCommentsAsync(postId, page, pageSize, cancellationToken);

        var responses = comments.Select(c => new GroupPostCommentResponse
        {
            Id = c.Id,
            AuthorUsername = c.User?.Username ?? string.Empty,
            AuthorAvatarUrl = c.User?.UserProfile?.AvatarUrl ?? string.Empty,
            Content = c.Content,
            CreatedAtUtc = c.CreatedAtUtc
        }).ToList();

        var pagedResult = new PagedResult<GroupPostCommentResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = comments.Count == pageSize ? page * pageSize + 1 : (page - 1) * pageSize + comments.Count
        };

        return GenericResult<PagedResult<GroupPostCommentResponse>>.Success(pagedResult);
    }
}
