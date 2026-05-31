using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MuseSpace.API.Hubs;

/// <summary>SignalR hub for real-time notification delivery.</summary>
[Authorize]
public class NotificationHub : Hub
{
    /// <summary>Called when a new client connects. Adds the user to their personal notification group.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        await base.OnConnectedAsync();
    }

    /// <summary>Called when a client disconnects. Removes the user from their personal notification group.</summary>
    /// <param name="exception">The exception that caused the disconnect, if any.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
