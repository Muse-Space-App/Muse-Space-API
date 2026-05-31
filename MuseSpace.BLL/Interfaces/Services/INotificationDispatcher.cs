using MuseSpace.BLL.DTO;

namespace MuseSpace.BLL.Interfaces.Services;

public interface INotificationDispatcher
{
    Task SendNotificationAsync(int userId, NotificationResponse notification, CancellationToken cancellationToken = default);
}
