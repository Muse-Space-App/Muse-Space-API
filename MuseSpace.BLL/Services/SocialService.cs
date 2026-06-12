using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Services;

public class SocialService : ISocialService
{
    private readonly ISocialRepository _socialRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public SocialService(ISocialRepository socialRepository, IUserRepository userRepository, IMapper mapper, INotificationService notificationService)
    {
        _socialRepository = socialRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<GenericResult<bool>> ToggleFollowAsync(int followerId, int followingId, CancellationToken cancellationToken = default)
    {
        if (followerId == followingId)
        {
            return GenericResult<bool>.Failure("You cannot follow yourself", ErrorType.ValidationFailed);
        }

        var followingUser = await _userRepository.GetByIdAsync(followingId, cancellationToken);
        if (followingUser == null)
        {
            return GenericResult<bool>.Failure("User not found", ErrorType.NotFound);
        }

        var isFollowing = await _socialRepository.IsFollowingAsync(followerId, followingId, cancellationToken);

        if (isFollowing)
        {
            var follow = new Follow { FollowerId = followerId, FollowingId = followingId };
            await _socialRepository.RemoveFollowAsync(follow, cancellationToken);
            return GenericResult<bool>.Success(false, "Unfollowed successfully");
        }
        else
        {
            var follow = new Follow { FollowerId = followerId, FollowingId = followingId };
            await _socialRepository.AddFollowAsync(follow, cancellationToken);

            // Trigger Notification
            await _notificationService.CreateNotificationAsync(
                followingId,
                "Follow",
                "Someone started following you.",
                $"/profile", // We can point to the follower's profile if we had their username, but this suffices.
                followerId,
                null,
                cancellationToken
            );

            return GenericResult<bool>.Success(true, "Followed successfully");
        }
    }

    public async Task<GenericResult<UserProfileResponse>> GetUserProfileAsync(int targetUserId, int? requestingUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(targetUserId, cancellationToken);
        if (user == null)
        {
            return GenericResult<UserProfileResponse>.Failure("User not found", ErrorType.NotFound);
        }

        var followerCount = await _socialRepository.GetFollowerCountAsync(targetUserId, cancellationToken);
        var followingCount = await _socialRepository.GetFollowingCountAsync(targetUserId, cancellationToken);

        bool isFollowing = false;
        if (requestingUserId.HasValue && requestingUserId.Value != targetUserId)
        {
            isFollowing = await _socialRepository.IsFollowingAsync(requestingUserId.Value, targetUserId, cancellationToken);
        }

        var response = new UserProfileResponse
        {
            UserId = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Bio = user.UserProfile?.Bio ?? string.Empty,
            AvatarUrl = user.UserProfile?.AvatarUrl ?? string.Empty,
            BannerUrl = user.UserProfile?.BannerUrl ?? string.Empty,
            CreatorTier = user.UserProfile?.CreatorTier ?? string.Empty,
            FollowerCount = followerCount,
            FollowingCount = followingCount,
            IsFollowing = isFollowing,
            IsAcceptingCommissions = user.UserProfile?.IsAcceptingCommissions ?? false
        };

        return GenericResult<UserProfileResponse>.Success(response);
    }

    public async Task<GenericResult<PagedResult<FollowerResponse>>> GetFollowersAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return GenericResult<PagedResult<FollowerResponse>>.Failure("User not found", ErrorType.NotFound);
        }

        var followers = await _socialRepository.GetFollowersAsync(userId, page, pageSize, cancellationToken);
        var totalCount = await _socialRepository.GetFollowerCountAsync(userId, cancellationToken);

        var responses = followers.Select(u => new FollowerResponse
        {
            UserId = u.Id,
            Username = u.Username,
            AvatarUrl = u.UserProfile?.AvatarUrl ?? string.Empty,
            Bio = u.UserProfile?.Bio ?? string.Empty
        }).ToList();

        var pagedResult = new PagedResult<FollowerResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return GenericResult<PagedResult<FollowerResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<PagedResult<FollowerResponse>>> GetFollowingAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return GenericResult<PagedResult<FollowerResponse>>.Failure("User not found", ErrorType.NotFound);
        }

        var following = await _socialRepository.GetFollowingAsync(userId, page, pageSize, cancellationToken);
        var totalCount = await _socialRepository.GetFollowingCountAsync(userId, cancellationToken);

        var responses = following.Select(u => new FollowerResponse
        {
            UserId = u.Id,
            Username = u.Username,
            AvatarUrl = u.UserProfile?.AvatarUrl ?? string.Empty,
            Bio = u.UserProfile?.Bio ?? string.Empty
        }).ToList();

        var pagedResult = new PagedResult<FollowerResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return GenericResult<PagedResult<FollowerResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<bool>> ToggleAcceptingCommissionsAsync(int userId, bool isAccepting, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.UserProfile == null)
        {
            return GenericResult<bool>.Failure("User profile not found", ErrorType.NotFound);
        }

        user.UserProfile.IsAcceptingCommissions = isAccepting;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return GenericResult<bool>.Success(isAccepting, isAccepting ? "You are now accepting commissions." : "You are no longer accepting commissions.");
    }

    public async Task<GenericResult<UserProfileResponse>> UpdateUserProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return GenericResult<UserProfileResponse>.Failure("User not found", ErrorType.NotFound);
        }

        if (user.UserProfile == null)
        {
            user.UserProfile = new UserProfile { UserId = userId };
        }

        if (request.Bio != null) user.UserProfile.Bio = request.Bio;
        if (request.AvatarUrl != null) user.UserProfile.AvatarUrl = request.AvatarUrl;
        if (request.BannerUrl != null) user.UserProfile.BannerUrl = request.BannerUrl;
        if (request.SocialLinks != null) user.UserProfile.SocialLinks = request.SocialLinks;

        await _userRepository.UpdateAsync(user, cancellationToken);

        return await GetUserProfileAsync(userId, userId, cancellationToken);
    }
}
