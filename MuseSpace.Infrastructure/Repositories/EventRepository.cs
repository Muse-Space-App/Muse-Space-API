using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    private readonly MuseSpaceDbContext _context;

    public EventRepository(MuseSpaceDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<Event?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(e => e.Organizer)
                .ThenInclude(u => u!.UserProfile)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> HasUserRsvpedAsync(int eventId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.EventRsvps
            .AnyAsync(er => er.EventId == eventId && er.UserId == userId, cancellationToken);
    }

    public async Task AddRsvpAsync(EventRsvp rsvp, CancellationToken cancellationToken = default)
    {
        await _context.EventRsvps.AddAsync(rsvp, cancellationToken);
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE \"Events\" SET \"RsvpCount\" = \"RsvpCount\" + 1 WHERE \"Id\" = {0}",
            new object[] { rsvp.EventId },
            cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRsvpAsync(EventRsvp rsvp, CancellationToken cancellationToken = default)
    {
        _context.EventRsvps.Remove(rsvp);
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE \"Events\" SET \"RsvpCount\" = CASE WHEN \"RsvpCount\" > 0 THEN \"RsvpCount\" - 1 ELSE 0 END WHERE \"Id\" = {0}",
            new object[] { rsvp.EventId },
            cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Event>> GetUpcomingEventsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.StartDateUtc >= DateTime.UtcNow)
            .Include(e => e.Organizer)
                .ThenInclude(u => u!.UserProfile)
            .OrderBy(e => e.StartDateUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Event>> GetEventsByOrganizerAsync(int organizerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.OrganizerId == organizerId)
            .Include(e => e.Organizer)
                .ThenInclude(u => u!.UserProfile)
            .OrderByDescending(e => e.StartDateUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<EventRsvp>> GetEventRsvpsAsync(int eventId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.EventRsvps
            .Where(er => er.EventId == eventId)
            .Include(er => er.User)
                .ThenInclude(u => u!.UserProfile)
            .OrderByDescending(er => er.RsvpedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Event>> GetRsvpedEventsByUserIdAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var eventIds = await _context.EventRsvps
            .Where(er => er.UserId == userId)
            .OrderByDescending(er => er.RsvpedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(er => er.EventId)
            .ToListAsync(cancellationToken);

        if (!eventIds.Any()) return new List<Event>();

        var events = await _context.Events
            .Include(e => e.Organizer)
                .ThenInclude(u => u!.UserProfile)
            .Where(e => eventIds.Contains(e.Id))
            .ToListAsync(cancellationToken);

        return eventIds.Select(id => events.FirstOrDefault(e => e.Id == id)).Where(e => e != null).Select(e => e!).ToList();
    }

    public async Task<int> GetUpcomingEventsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Events.CountAsync(e => e.StartDateUtc >= DateTime.UtcNow, cancellationToken);
    }

    public async Task<int> GetEventsByOrganizerCountAsync(int organizerId, CancellationToken cancellationToken = default)
    {
        return await _context.Events.CountAsync(e => e.OrganizerId == organizerId, cancellationToken);
    }

    public async Task<int> GetEventRsvpCountAsync(int eventId, CancellationToken cancellationToken = default)
    {
        return await _context.EventRsvps.CountAsync(er => er.EventId == eventId, cancellationToken);
    }
}
