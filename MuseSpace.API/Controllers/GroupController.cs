using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;
using System.Security.Claims;

using Microsoft.AspNetCore.SignalR;
using MuseSpace.API.Hubs;

namespace MuseSpace.API.Controllers;

/// <summary>Controller for managing group operations.</summary>
[ApiController]
[Route("api/groups")]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly IHubContext<GroupChatHub> _hubContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupController"/> class.
    /// </summary>
    /// <param name="groupService">The group service.</param>
    /// <param name="hubContext">The SignalR Hub context for group chat.</param>
    public GroupController(IGroupService groupService, IHubContext<GroupChatHub> hubContext)
    {
        _groupService = groupService;
        _hubContext = hubContext;
    }

    /// <summary>Creates a new group.</summary>
    /// <param name="request">The group creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created group.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<GroupResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _groupService.CreateGroupAsync(userId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets a group by its identifier.</summary>
    /// <param name="groupId">The group identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The group details.</returns>
    [HttpGet("{groupId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<GroupResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroup(int groupId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : null;
        var result = await _groupService.GetGroupAsync(groupId, currentUserId, cancellationToken);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Gets all groups (paged).</summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged groups</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<GroupResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllGroups([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _groupService.GetAllGroupsAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing group.</summary>
    /// <param name="groupId">The group identifier.</param>
    /// <param name="request">The group update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated group.</returns>
    [HttpPut("{groupId}")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<GroupResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _groupService.UpdateGroupAsync(groupId, userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>Joins the current user to a group.</summary>
    /// <param name="groupId">The group identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A value indicating whether the join succeeded.</returns>
    [HttpPost("{groupId}/join")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> JoinGroup(int groupId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _groupService.JoinGroupAsync(groupId, userId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>Removes the current user from a group.</summary>
    /// <param name="groupId">The group identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A value indicating whether the leave succeeded.</returns>
    [HttpPost("{groupId}/leave")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeaveGroup(int groupId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _groupService.LeaveGroupAsync(groupId, userId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>Gets the members of a group.</summary>
    /// <param name="groupId">The group identifier.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of group members.</returns>
    [HttpGet("{groupId}/members")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<GroupMemberResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMembers(int groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _groupService.GetGroupMembersAsync(groupId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new post in a group.</summary>
    /// <param name="groupId">The group identifier.</param>
    /// <param name="request">The post creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created group post.</returns>
    [HttpPost("{groupId}/posts")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<GroupPostResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePost(int groupId, [FromBody] CreateGroupPostRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _groupService.CreateGroupPostAsync(groupId, userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }

        // Broadcast the new post to connected clients in the group room
        await _hubContext.Clients.Group($"Group_{groupId}").SendAsync("ReceiveMessage", result.Data, cancellationToken);

        return Ok(result);
    }

    /// <summary>Gets posts in a group.</summary>
    /// <param name="groupId">The group identifier.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of group posts.</returns>
    [HttpGet("{groupId}/posts")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<GroupPostResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts(int groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _groupService.GetGroupPostsAsync(groupId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Deletes a group post.</summary>
    /// <param name="postId">The post identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A value indicating whether the deletion succeeded.</returns>
    [HttpDelete("posts/{postId}")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletePost(int postId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _groupService.DeleteGroupPostAsync(postId, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            if (result.ErrorType == ErrorType.NotFound) return NotFound();
            return BadRequest(result);
        }
        return Ok(result);
    }
}
