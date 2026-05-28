using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Endpoints for interacting with artworks (Like, Bookmark, Share).
/// </summary>
[ApiController]
[Route("api/artworks")]
public class InteractionController : ControllerBase
{
    private readonly IInteractionService _interactionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionController"/> class.
    /// </summary>
    /// <param name="interactionService">The interaction service.</param>
    public InteractionController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    /// <summary>
    /// Toggle Like on an artwork
    /// </summary>
    /// <param name="artworkId">The ID of the artwork</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if liked, false if unliked</returns>
    [HttpPost("{artworkId}/like")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleLike(int artworkId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _interactionService.ToggleLikeAsync(artworkId, userId, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Toggle Bookmark on an artwork
    /// </summary>
    /// <param name="artworkId">The ID of the artwork</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if bookmarked, false if removed from bookmarks</returns>
    [HttpPost("{artworkId}/bookmark")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleBookmark(int artworkId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _interactionService.ToggleBookmarkAsync(artworkId, userId, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Record an artwork share
    /// </summary>
    /// <param name="artworkId">The ID of the artwork</param>
    /// <param name="request">The platform the artwork was shared to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{artworkId}/share")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Share(int artworkId, [FromBody] ShareArtworkRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _interactionService.RecordShareAsync(artworkId, userId, request.Platform, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
