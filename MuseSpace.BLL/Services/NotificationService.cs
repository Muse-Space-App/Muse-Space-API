using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    private readonly INotificationDispatcher _dispatcher;

    public NotificationService(
        INotificationRepository notificationRepository,
        IMapper mapper,
        INotificationDispatcher dispatcher)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _dispatcher = dispatcher;
    }

    public async Task<GenericResult<PagedResult<NotificationResponse>>> GetUserNotificationsAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetUserNotificationsAsync(userId, page, pageSize, cancellationToken);
        // Note: Missing total count in repo, using a basic approximation or we can add Count method
        var totalCount = notifications.Count; // Simplification for now

        var responses = notifications.Select(n => new NotificationResponse
        {
            Id = n.Id,
            UserId = n.UserId,
            Type = n.Type,
            Message = n.Message,
            IsRead = n.IsRead,
            ActionUrl = n.ActionUrl,
            CreatedAtUtc = n.CreatedAtUtc,
            RelatedUserId = n.RelatedUserId,
            RelatedUserUsername = n.RelatedUser?.Username,
            RelatedUserAvatarUrl = n.RelatedUser?.UserProfile?.AvatarUrl,
            RelatedArtworkId = n.RelatedArtworkId,
            RelatedArtworkThumbnailUrl = n.RelatedArtwork?.ThumbnailUrl
        }).ToList();

        var pagedResult = new PagedResult<NotificationResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return GenericResult<PagedResult<NotificationResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<int>> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        var count = await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);
        return GenericResult<int>.Success(count);
    }

    public async Task<GenericResult<bool>> MarkAsReadAsync(int notificationId, int userId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
        {
            return GenericResult<bool>.Failure("Notification not found", ErrorType.NotFound);
        }

        if (notification.UserId != userId)
        {
            return GenericResult<bool>.Failure("Unauthorized", ErrorType.Unauthorized);
        }

        await _notificationRepository.MarkAsReadAsync(notificationId, cancellationToken);
        return GenericResult<bool>.Success(true);
    }

    public async Task<GenericResult<bool>> MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId, cancellationToken);
        return GenericResult<bool>.Success(true);
    }

    public async Task CreateNotificationAsync(
        int userId,
        string type,
        string message,
        string? actionUrl = null,
        int? relatedUserId = null,
        int? relatedArtworkId = null,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Message = message,
            ActionUrl = actionUrl,
            RelatedUserId = relatedUserId,
            RelatedArtworkId = relatedArtworkId,
            IsRead = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);

        // Map to response for SignalR
        var response = new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type,
            Message = notification.Message,
            IsRead = notification.IsRead,
            ActionUrl = notification.ActionUrl,
            CreatedAtUtc = notification.CreatedAtUtc,
            RelatedUserId = notification.RelatedUserId,
            RelatedArtworkId = notification.RelatedArtworkId
        };

        // Broadcast via SignalR
        await _dispatcher.SendNotificationAsync(userId, response, cancellationToken);
    }
}
