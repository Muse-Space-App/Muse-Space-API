using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Endpoints for retrieving personalized and trending artwork feeds.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FeedController : ControllerBase
{
    private readonly IArtworkService _artworkService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedController"/> class.
    /// </summary>
    /// <param name="artworkService">The artwork service.</param>
    public FeedController(IArtworkService artworkService)
    {
        _artworkService = artworkService;
    }

    /// <summary>
    /// Get the artwork feed
    /// </summary>
    /// <remarks>
    /// Retrieves a paginated feed of artworks, customized if the user is authenticated.
    /// </remarks>
    /// <param name="cursor">Optional cursor for pagination (the ID of the last fetched artwork)</param>
    /// <param name="limit">The maximum number of artworks to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A feed of artworks and a boolean indicating if there are more</returns>
    /// <response code="200">Returns the artwork feed</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<ArtworkFeedResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeed([FromQuery] int? cursor, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? currentUserId = string.IsNullOrEmpty(currentUserIdClaim) ? null : int.Parse(currentUserIdClaim);

        var result = await _artworkService.GetFeedAsync(cursor, limit, currentUserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get trending artworks
    /// </summary>
    /// <remarks>
    /// Retrieves a list of trending artworks based on recent views, likes, and interactions.
    /// </remarks>
    /// <param name="limit">The maximum number of trending artworks to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of trending artworks</returns>
    /// <response code="200">Returns the trending artworks</response>
    [HttpGet("trending")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<IReadOnlyCollection<ArtworkResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrending([FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? currentUserId = string.IsNullOrEmpty(currentUserIdClaim) ? null : int.Parse(currentUserIdClaim);

        var result = await _artworkService.GetTrendingAsync(limit, currentUserId, cancellationToken);
        return Ok(result);
    }
}
