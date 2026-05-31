using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>Controller for managing notification operations.</summary>
[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    /// <summary>Initializes a new instance of the <see cref="NotificationController"/> class.</summary>
    /// <param name="notificationService">The notification service.</param>
    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>Gets the current user's notifications.</summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of notifications.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GenericResult<PagedResult<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _notificationService.GetUserNotificationsAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets the count of unread notifications for the current user.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The unread notification count.</returns>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(GenericResult<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _notificationService.GetUnreadCountAsync(userId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Marks a specific notification as read.</summary>
    /// <param name="notificationId">The notification identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    [HttpPost("{notificationId}/read")]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead(int notificationId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _notificationService.MarkAsReadAsync(notificationId, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound();
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>Marks all notifications as read for the current user.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    [HttpPost("read-all")]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
        return Ok(result);
    }
}
