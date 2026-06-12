using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface IEventRepository : IRepository<Event>
{
    Task<bool> HasUserRsvpedAsync(int eventId, int userId, CancellationToken cancellationToken = default);
    Task AddRsvpAsync(EventRsvp rsvp, CancellationToken cancellationToken = default);
    Task RemoveRsvpAsync(EventRsvp rsvp, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Event>> GetUpcomingEventsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Event>> GetEventsByOrganizerAsync(int organizerId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<EventRsvp>> GetEventRsvpsAsync(int eventId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Event>> GetRsvpedEventsByUserIdAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<int> GetUpcomingEventsCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetEventsByOrganizerCountAsync(int organizerId, CancellationToken cancellationToken = default);
    Task<int> GetEventRsvpCountAsync(int eventId, CancellationToken cancellationToken = default);
}
