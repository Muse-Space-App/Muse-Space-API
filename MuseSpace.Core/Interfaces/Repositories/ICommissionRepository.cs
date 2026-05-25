using MuseSpace.Core.Entities;
using MuseSpace.Core.Enums;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface ICommissionRepository : IRepository<Commission>
{
    Task<IReadOnlyCollection<Commission>> GetCommissionsByRequesterAsync(int requesterId, int skip, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Commission>> GetCommissionsByArtistAsync(int artistId, int skip, int take, CancellationToken cancellationToken = default);
    Task<Commission?> GetCommissionWithDetailsAsync(int commissionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CommissionMessage>> GetCommissionMessagesAsync(int commissionId, int skip, int take, CancellationToken cancellationToken = default);
    Task AddMessageAsync(CommissionMessage message, CancellationToken cancellationToken = default);
    Task MarkMessagesAsReadAsync(int commissionId, int userId, CancellationToken cancellationToken = default);
}
