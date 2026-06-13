using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface IEventService
{
    Task<GenericResult<EventResponse>> CreateEventAsync(int userId, CreateEventRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<EventResponse>> UpdateEventAsync(int eventId, int userId, UpdateEventRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> DeleteEventAsync(int eventId, int userId, CancellationToken cancellationToken = default);

    Task<GenericResult<EventResponse>> GetEventAsync(int eventId, int? currentUserId = null, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<EventResponse>>> GetUpcomingEventsAsync(int page, int pageSize, int? currentUserId = null, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<EventResponse>>> GetEventsByOrganizerAsync(int organizerId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<GenericResult<bool>> RsvpEventAsync(int eventId, int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> CancelRsvpAsync(int eventId, int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<EventRsvpResponse>>> GetEventRsvpsAsync(int eventId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<EventResponse>>> GetMyRsvpedEventsAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
}
