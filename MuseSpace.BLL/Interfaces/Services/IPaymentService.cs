using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface IPaymentService
{
    Task<GenericResult<string>> GeneratePaymentQrUrlAsync(int commissionId, int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> ConfirmPaymentAsync(int commissionId, int userId, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> VerifyPaymentAsync(int commissionId, int artistId, CancellationToken cancellationToken = default);
}
