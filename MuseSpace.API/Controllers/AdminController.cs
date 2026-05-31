using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Handles admin and moderation actions.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminController"/> class.
    /// </summary>
    /// <param name="adminService">The admin service.</param>
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    private int GetCurrentUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdString, out int userId) ? userId : 0;
    }

    /// <summary>
    /// Gets aggregated admin statistics.
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(GenericResult<AdminStatsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await _adminService.GetAdminStatsAsync(cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of pending reports.
    /// </summary>
    [HttpGet("reports")]
    [ProducesResponseType(typeof(GenericResult<PagedResult<ReportResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _adminService.GetPendingReportsAsync(page, pageSize, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Reviews and actions a specific report.
    /// </summary>
    [HttpPost("reports/{reportId}/review")]
    [ProducesResponseType(typeof(GenericResult<ReportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReviewReport(int reportId, [FromBody] ReviewReportRequest request, CancellationToken cancellationToken)
    {
        var adminId = GetCurrentUserId();
        var result = await _adminService.ReviewReportAsync(reportId, adminId, request, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Bans a user.
    /// </summary>
    [HttpPost("users/ban")]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BanUser([FromBody] BanUserRequest request, CancellationToken cancellationToken)
    {
        var adminId = GetCurrentUserId();
        var result = await _adminService.BanUserAsync(request, adminId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Unbans a user.
    /// </summary>
    [HttpPost("users/{userId}/unban")]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnbanUser(int userId, CancellationToken cancellationToken)
    {
        var adminId = GetCurrentUserId();
        var result = await _adminService.UnbanUserAsync(userId, adminId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
