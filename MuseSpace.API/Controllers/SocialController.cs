using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>Controller for managing social operations.</summary>
[ApiController]
public class SocialController : ControllerBase
{
    private readonly ISocialService _socialService;

    /// <summary>Initializes a new instance of the <see cref="SocialController"/> class.</summary>
    /// <param name="socialService">The social service.</param>
    public SocialController(ISocialService socialService)
    {
        _socialService = socialService;
    }

    /// <summary>
    /// Follow or Unfollow a user
    /// </summary>
    [HttpPost("api/users/{userId}/follow")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleFollow(int userId, CancellationToken cancellationToken)
    {
        var followerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _socialService.ToggleFollowAsync(followerId, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Get a user's profile
    /// </summary>
    [HttpGet("api/users/{userId}/profile")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<UserProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(int userId, CancellationToken cancellationToken)
    {
        int? requestingUserId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        var result = await _socialService.GetUserProfileAsync(userId, requestingUserId, cancellationToken);
        if (!result.IsSuccess) return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Update the current user's profile
    /// </summary>
    [HttpPut("api/users/profile")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<UserProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _socialService.UpdateUserProfileAsync(userId, request, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get a user's followers
    /// </summary>
    [HttpGet("api/users/{userId}/followers")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<FollowerResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowers(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _socialService.GetFollowersAsync(userId, page, pageSize, cancellationToken);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Get the users that a user is following
    /// </summary>
    [HttpGet("api/users/{userId}/following")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<FollowerResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowing(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _socialService.GetFollowingAsync(userId, page, pageSize, cancellationToken);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Toggle the user's commission accepting status
    /// </summary>
    [HttpPut("api/users/profile/commissions-status")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleAcceptingCommissions([FromBody] ToggleAcceptingCommissionsRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _socialService.ToggleAcceptingCommissionsAsync(userId, request.IsAcceptingCommissions, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
