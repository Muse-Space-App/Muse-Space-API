using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>Controller for managing artwork recommendation operations.</summary>
[ApiController]
[Route("api/recommendations")]
[Authorize]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    /// <summary>Initializes a new instance of the <see cref="RecommendationController"/> class.</summary>
    /// <param name="recommendationService">The recommendation service.</param>
    public RecommendationController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    /// <summary>Gets personalized artwork recommendations for the current user.</summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of recommended artworks.</returns>
    [HttpGet]
    [EnableRateLimiting("fixed")]
    [ProducesResponseType(typeof(GenericResult<PagedResult<ArtworkResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecommendations([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _recommendationService.GetRecommendedArtworksAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets similar artworks based on the target artwork.
    /// </summary>
    /// <param name="artworkId">Target artwork ID</param>
    /// <param name="page">The page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Similar artworks</returns>
    [HttpGet("similar/{artworkId}")]
    [ProducesResponseType(typeof(GenericResult<PagedResult<ArtworkResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSimilarArtworks(int artworkId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _recommendationService.GetSimilarArtworksAsync(artworkId, page, pageSize, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
