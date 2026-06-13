using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>Controller for managing event operations.</summary>
[ApiController]
[Route("api/events")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    /// <summary>Initializes a new instance of the <see cref="EventController"/> class.</summary>
    /// <param name="eventService">The event service.</param>
    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>Creates a new event.</summary>
    /// <param name="request">The event creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created event.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<EventResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _eventService.CreateEventAsync(userId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets an event by its identifier.</summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The event details.</returns>
    [HttpGet("{eventId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<EventResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvent(int eventId, CancellationToken cancellationToken)
    {
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                userId = int.Parse(userIdClaim);
            }
        }

        var result = await _eventService.GetEventAsync(eventId, userId, cancellationToken);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Updates an existing event.</summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="request">The event update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated event.</returns>
    [HttpPut("{eventId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<EventResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateEvent(int eventId, [FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _eventService.UpdateEventAsync(eventId, userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            if (result.ErrorType == ErrorType.NotFound) return NotFound();
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>Deletes an event.</summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A value indicating whether the deletion succeeded.</returns>
    [HttpDelete("{eventId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteEvent(int eventId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _eventService.DeleteEventAsync(eventId, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            if (result.ErrorType == ErrorType.NotFound) return NotFound();
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>Gets a paged list of upcoming events.</summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of upcoming events.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<EventResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                userId = int.Parse(userIdClaim);
            }
        }

        var result = await _eventService.GetUpcomingEventsAsync(page, pageSize, userId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets events created by a specific organizer.</summary>
    /// <param name="organizerId">The organizer's user identifier.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of events by the organizer.</returns>
    [HttpGet("organizer/{organizerId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<EventResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventsByOrganizer(int organizerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _eventService.GetEventsByOrganizerAsync(organizerId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>RSVPs the current user to an event.</summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A value indicating whether the RSVP succeeded.</returns>
    [HttpPost("{eventId:int}/rsvp")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RsvpEvent(int eventId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _eventService.RsvpEventAsync(eventId, userId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>Cancels the current user's RSVP to an event.</summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A value indicating whether the cancellation succeeded.</returns>
    [HttpDelete("{eventId:int}/rsvp")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelRsvp(int eventId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _eventService.CancelRsvpAsync(eventId, userId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>Gets the list of RSVPs for an event.</summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of event RSVPs.</returns>
    [HttpGet("{eventId:int}/rsvps")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<EventRsvpResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventRsvps(int eventId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _eventService.GetEventRsvpsAsync(eventId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets the list of events the current user has RSVP'd to.</summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of events.</returns>
    [HttpGet("my-rsvps")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<PagedResult<EventResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyRsvpedEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _eventService.GetMyRsvpedEventsAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }
}
