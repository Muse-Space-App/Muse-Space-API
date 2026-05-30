using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;
using System.Web;

namespace MuseSpace.BLL.Services;

public class QrisPaymentService : IPaymentService
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly INotificationService _notificationService;

    public QrisPaymentService(
        ICommissionRepository commissionRepository,
        INotificationService notificationService)
    {
        _commissionRepository = commissionRepository;
        _notificationService = notificationService;
    }

    public async Task<GenericResult<string>> GeneratePaymentQrUrlAsync(int commissionId, int userId, CancellationToken cancellationToken = default)
    {
        var commission = await _commissionRepository.GetByIdAsync(commissionId, cancellationToken);
        if (commission == null)
            return GenericResult<string>.Failure("Commission not found.", ErrorType.NotFound);

        if (commission.RequesterId != userId)
            return GenericResult<string>.Failure("You are not the requester for this commission.", ErrorType.Unauthorized);

        if (commission.Status != CommissionStatus.Accepted)
            return GenericResult<string>.Failure("Commission must be Accepted before payment can be generated.", ErrorType.ValidationFailed);

        // Generate QR Code URL via free API
        string payload = $"Commission #{commission.Id} - {commission.Title} - Rp{commission.Price}";
        string qrUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={HttpUtility.UrlEncode(payload)}";

        // Update status indicating client is in payment step
        commission.Status = CommissionStatus.PendingVerification;
        commission.UpdatedAtUtc = DateTime.UtcNow;

        await _commissionRepository.UpdateAsync(commission, cancellationToken);

        return GenericResult<string>.Success(qrUrl);
    }

    public async Task<GenericResult<bool>> ConfirmPaymentAsync(int commissionId, int userId, CancellationToken cancellationToken = default)
    {
        var commission = await _commissionRepository.GetByIdAsync(commissionId, cancellationToken);
        if (commission == null)
            return GenericResult<bool>.Failure("Commission not found.", ErrorType.NotFound);

        if (commission.RequesterId != userId)
            return GenericResult<bool>.Failure("You are not the requester for this commission.", ErrorType.Unauthorized);

        if (commission.Status != CommissionStatus.PendingVerification)
            return GenericResult<bool>.Failure("Commission is not in payment verification state.", ErrorType.ValidationFailed);

        // Notify Artist that client claims to have paid
        await _notificationService.CreateNotificationAsync(
            commission.ArtistId,
            "PaymentSubmitted",
            "A client has submitted payment for a commission. Please verify receipt.",
            null,
            userId,
            null,
            cancellationToken
        );

        return GenericResult<bool>.Success(true);
    }

    public async Task<GenericResult<bool>> VerifyPaymentAsync(int commissionId, int artistId, CancellationToken cancellationToken = default)
    {
        var commission = await _commissionRepository.GetByIdAsync(commissionId, cancellationToken);
        if (commission == null)
            return GenericResult<bool>.Failure("Commission not found.", ErrorType.NotFound);

        if (commission.ArtistId != artistId)
            return GenericResult<bool>.Failure("You are not the artist for this commission.", ErrorType.Unauthorized);

        if (commission.Status != CommissionStatus.PendingVerification)
            return GenericResult<bool>.Failure("Commission is not pending payment verification.", ErrorType.ValidationFailed);

        commission.Status = CommissionStatus.InProgress;
        commission.UpdatedAtUtc = DateTime.UtcNow;

        await _commissionRepository.UpdateAsync(commission, cancellationToken);

        // Notify Requester
        await _notificationService.CreateNotificationAsync(
            commission.RequesterId,
            "PaymentVerified",
            "Your payment has been verified. The commission is now In Progress!",
            null,
            artistId,
            null,
            cancellationToken
        );

        return GenericResult<bool>.Success(true);
    }
}
