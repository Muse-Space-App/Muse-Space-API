using Microsoft.AspNetCore.SignalR;
using MuseSpace.API.Hubs;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;

namespace MuseSpace.API.Services;

/// <summary>Dispatches notifications to connected clients via SignalR.</summary>
public class SignalRNotificationDispatcher : INotificationDispatcher
{
    private readonly IHubContext<NotificationHub> _hubContext;

    /// <summary>Initializes a new instance of the <see cref="SignalRNotificationDispatcher"/> class.</summary>
    /// <param name="hubContext">The SignalR hub context used to send messages to connected clients.</param>
    public SignalRNotificationDispatcher(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>Sends a notification to a specific user's SignalR group.</summary>
    /// <param name="userId">The identifier of the user to receive the notification.</param>
    /// <param name="notification">The notification payload to send.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task SendNotificationAsync(int userId, NotificationResponse notification, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", notification, cancellationToken);
    }
}
