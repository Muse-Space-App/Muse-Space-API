using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Handles mock QRIS payment generation and verification for commissions.
/// </summary>
[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentController"/> class.
    /// </summary>
    /// <param name="paymentService">The payment service.</param>
    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    private int GetCurrentUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdString, out int userId) ? userId : 0;
    }

    /// <summary>
    /// Generates a mock QRIS code URL for a commission.
    /// </summary>
    [HttpGet("{commissionId}/qr")]
    [ProducesResponseType(typeof(GenericResult<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateQr(int commissionId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _paymentService.GeneratePaymentQrUrlAsync(commissionId, userId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Confirms that the client has paid.
    /// </summary>
    [HttpPost("{commissionId}/confirm")]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmPayment(int commissionId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _paymentService.ConfirmPaymentAsync(commissionId, userId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Verifies the payment was received (by the artist).
    /// </summary>
    [HttpPost("{commissionId}/verify")]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyPayment(int commissionId, CancellationToken cancellationToken)
    {
        var artistId = GetCurrentUserId();
        var result = await _paymentService.VerifyPaymentAsync(commissionId, artistId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
