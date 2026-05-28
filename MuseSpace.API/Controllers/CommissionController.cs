using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Handles commission requests, status updates, and messaging between artists and clients.
/// </summary>
[ApiController]
[Route("api/commissions")]
[Authorize]
public class CommissionController : ControllerBase
{
    private readonly ICommissionService _commissionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommissionController"/> class.
    /// </summary>
    /// <param name="commissionService">The commission service.</param>
    public CommissionController(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    /// <summary>
    /// Creates a new commission request.
    /// </summary>
    /// <param name="request">Commission details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created commission</returns>
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [ProducesResponseType(typeof(GenericResult<CommissionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCommission([FromBody] CreateCommissionRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commissionService.CreateCommissionAsync(userId, request, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Gets commissions requested by the current user.
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged list of requested commissions</returns>
    [HttpGet("requested")]
    [ProducesResponseType(typeof(GenericResult<PagedResult<CommissionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRequestedCommissions([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commissionService.GetCommissionsByRequesterAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets commissions received by the current user (as an artist).
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged list of received commissions</returns>
    [HttpGet("received")]
    [ProducesResponseType(typeof(GenericResult<PagedResult<CommissionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReceivedCommissions([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commissionService.GetCommissionsByArtistAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets details of a specific commission.
    /// </summary>
    /// <param name="commissionId">Commission ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Commission details</returns>
    [HttpGet("{commissionId}")]
    [ProducesResponseType(typeof(GenericResult<CommissionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCommission(int commissionId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commissionService.GetCommissionAsync(commissionId, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Updates the status of a commission (e.g., Accept, Reject, Complete).
    /// </summary>
    /// <param name="commissionId">Commission ID</param>
    /// <param name="request">New status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated commission</returns>
    [HttpPatch("{commissionId}/status")]
    [ProducesResponseType(typeof(GenericResult<CommissionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(int commissionId, [FromBody] UpdateCommissionStatusRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commissionService.UpdateCommissionStatusAsync(commissionId, userId, request, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            if (result.ErrorType == ErrorType.Forbidden) return Forbid();
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Gets messages for a specific commission.
    /// </summary>
    /// <param name="commissionId">Commission ID</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged list of messages</returns>
    [HttpGet("{commissionId}/messages")]
    [ProducesResponseType(typeof(GenericResult<PagedResult<CommissionMessageResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(int commissionId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commissionService.GetCommissionMessagesAsync(commissionId, userId, page, pageSize, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Sends a message in a commission thread.
    /// </summary>
    /// <param name="commissionId">Commission ID</param>
    /// <param name="request">Message content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sent message</returns>
    [HttpPost("{commissionId}/messages")]
    [EnableRateLimiting("fixed")]
    [ProducesResponseType(typeof(GenericResult<CommissionMessageResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMessage(int commissionId, [FromBody] CreateCommissionMessageRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commissionService.SendMessageAsync(commissionId, userId, request, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }
        return Ok(result);
    }
}
