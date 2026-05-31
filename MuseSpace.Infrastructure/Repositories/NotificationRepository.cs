using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    private readonly MuseSpaceDbContext _context;

    public NotificationRepository(MuseSpaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Notification>> GetUserNotificationsAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .Include(n => n.RelatedUser)
                .ThenInclude(u => u!.UserProfile)
            .Include(n => n.RelatedArtwork)
            .OrderByDescending(n => n.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }

    public async Task MarkAsReadAsync(int notificationId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE Notifications SET IsRead = true WHERE Id = {0}",
            new object[] { notificationId },
            cancellationToken);
    }

    public async Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE Notifications SET IsRead = true WHERE UserId = {0} AND IsRead = false",
            new object[] { userId },
            cancellationToken);
    }
}
