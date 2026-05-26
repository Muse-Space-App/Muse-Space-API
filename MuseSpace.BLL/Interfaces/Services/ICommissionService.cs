using MuseSpace.BLL.DTO;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface ICommissionService
{
    Task<GenericResult<CommissionResponse>> CreateCommissionAsync(int requesterId, CreateCommissionRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<CommissionResponse>>> GetCommissionsByRequesterAsync(int requesterId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<CommissionResponse>>> GetCommissionsByArtistAsync(int artistId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<CommissionResponse>> GetCommissionAsync(int commissionId, int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<CommissionResponse>> UpdateCommissionStatusAsync(int commissionId, int userId, UpdateCommissionStatusRequest request, CancellationToken cancellationToken = default);

    Task<GenericResult<PagedResult<CommissionMessageResponse>>> GetCommissionMessagesAsync(int commissionId, int userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<CommissionMessageResponse>> SendMessageAsync(int commissionId, int senderId, CreateCommissionMessageRequest request, CancellationToken cancellationToken = default);
}
