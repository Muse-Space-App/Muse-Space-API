using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;

    public EventService(IEventRepository eventRepository, IMapper mapper)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
    }

    public async Task<GenericResult<EventResponse>> CreateEventAsync(int userId, CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        var @event = new Event
        {
            Title = request.Title,
            Description = request.Description,
            BannerUrl = request.BannerUrl,
            StartDateUtc = request.StartDateUtc,
            EndDateUtc = request.EndDateUtc,
            Location = request.Location,
            IsOnline = request.IsOnline,
            EventUrl = request.EventUrl,
            OrganizerId = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(@event, cancellationToken);
        var response = _mapper.Map<EventResponse>(@event);
        return GenericResult<EventResponse>.Success(response, "Event created successfully");
    }

    public async Task<GenericResult<EventResponse>> UpdateEventAsync(int eventId, int userId, UpdateEventRequest request, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
        {
            return GenericResult<EventResponse>.Failure("Event not found", ErrorType.NotFound);
        }

        if (@event.OrganizerId != userId)
        {
            return GenericResult<EventResponse>.Failure("Only the organizer can update the event", ErrorType.Unauthorized);
        }

        @event.Title = request.Title;
        @event.Description = request.Description;
        @event.BannerUrl = request.BannerUrl;
        @event.StartDateUtc = request.StartDateUtc;
        @event.EndDateUtc = request.EndDateUtc;
        @event.Location = request.Location;
        @event.IsOnline = request.IsOnline;
        @event.EventUrl = request.EventUrl;

        await _eventRepository.UpdateAsync(@event, cancellationToken);
        var response = _mapper.Map<EventResponse>(@event);
        return GenericResult<EventResponse>.Success(response, "Event updated successfully");
    }

    public async Task<GenericResult<bool>> DeleteEventAsync(int eventId, int userId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
        {
            return GenericResult<bool>.Failure("Event not found", ErrorType.NotFound);
        }

        if (@event.OrganizerId != userId)
        {
            return GenericResult<bool>.Failure("Only the organizer can delete the event", ErrorType.Unauthorized);
        }

        await _eventRepository.DeleteAsync(@event.Id, cancellationToken);
        return GenericResult<bool>.Success(true, "Event deleted successfully");
    }

    public async Task<GenericResult<EventResponse>> GetEventAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
        {
            return GenericResult<EventResponse>.Failure("Event not found", ErrorType.NotFound);
        }

        var response = _mapper.Map<EventResponse>(@event);
        return GenericResult<EventResponse>.Success(response);
    }

    public async Task<GenericResult<PagedResult<EventResponse>>> GetUpcomingEventsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetUpcomingEventsAsync(page, pageSize, cancellationToken);
        var count = await _eventRepository.GetUpcomingEventsCountAsync(cancellationToken);

        var responses = _mapper.Map<IReadOnlyCollection<EventResponse>>(events);
        var pagedResult = new PagedResult<EventResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = count
        };

        return GenericResult<PagedResult<EventResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<PagedResult<EventResponse>>> GetEventsByOrganizerAsync(int organizerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetEventsByOrganizerAsync(organizerId, page, pageSize, cancellationToken);
        var count = await _eventRepository.GetEventsByOrganizerCountAsync(organizerId, cancellationToken);

        var responses = _mapper.Map<IReadOnlyCollection<EventResponse>>(events);
        var pagedResult = new PagedResult<EventResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = count
        };

        return GenericResult<PagedResult<EventResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<bool>> RsvpEventAsync(int eventId, int userId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
        {
            return GenericResult<bool>.Failure("Event not found", ErrorType.NotFound);
        }

        var hasRsvped = await _eventRepository.HasUserRsvpedAsync(eventId, userId, cancellationToken);
        if (hasRsvped)
        {
            return GenericResult<bool>.Failure("You have already RSVPed to this event", ErrorType.ValidationFailed);
        }

        var rsvp = new EventRsvp
        {
            EventId = eventId,
            UserId = userId,
            RsvpedAtUtc = DateTime.UtcNow
        };

        await _eventRepository.AddRsvpAsync(rsvp, cancellationToken);
        return GenericResult<bool>.Success(true, "RSVPed successfully");
    }

    public async Task<GenericResult<bool>> CancelRsvpAsync(int eventId, int userId, CancellationToken cancellationToken = default)
    {
        var rsvp = new EventRsvp { EventId = eventId, UserId = userId }; // Note: needs attached object or Get first

        var hasRsvped = await _eventRepository.HasUserRsvpedAsync(eventId, userId, cancellationToken);
        if (!hasRsvped)
        {
            return GenericResult<bool>.Failure("You have not RSVPed to this event", ErrorType.ValidationFailed);
        }

        await _eventRepository.RemoveRsvpAsync(rsvp, cancellationToken);
        return GenericResult<bool>.Success(true, "RSVP cancelled successfully");
    }

    public async Task<GenericResult<PagedResult<EventRsvpResponse>>> GetEventRsvpsAsync(int eventId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var rsvps = await _eventRepository.GetEventRsvpsAsync(eventId, page, pageSize, cancellationToken);
        var count = await _eventRepository.GetEventRsvpCountAsync(eventId, cancellationToken);

        var responses = rsvps.Select(r => new EventRsvpResponse
        {
            EventId = r.EventId,
            UserId = r.UserId,
            Username = r.User?.Username ?? string.Empty,
            AvatarUrl = r.User?.UserProfile?.AvatarUrl ?? string.Empty,
            RsvpedAtUtc = r.RsvpedAtUtc
        }).ToList();

        var pagedResult = new PagedResult<EventRsvpResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = count
        };

        return GenericResult<PagedResult<EventRsvpResponse>>.Success(pagedResult);
    }
}
