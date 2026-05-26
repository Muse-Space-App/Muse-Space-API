using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface INotificationService
{
    Task<GenericResult<PagedResult<NotificationResponse>>> GetUserNotificationsAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<int>> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> MarkAsReadAsync(int notificationId, int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default);

    // Internal use for triggering notifications
    Task CreateNotificationAsync(int userId, string type, string message, string? actionUrl = null, int? relatedUserId = null, int? relatedArtworkId = null, CancellationToken cancellationToken = default);
}
