using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Results;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Endpoints for retrieving and managing tags associated with artworks.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagController"/> class.
    /// </summary>
    /// <param name="tagService">The tag service.</param>
    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    /// <summary>
    /// Get popular tags
    /// </summary>
    /// <remarks>
    /// Retrieves a list of the most popular tags used across all artworks.
    /// </remarks>
    /// <param name="limit">The maximum number of tags to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of popular tags</returns>
    /// <response code="200">Returns the popular tags</response>
    [HttpGet("popular")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<IReadOnlyCollection<TagResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopular([FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var result = await _tagService.GetPopularAsync(limit, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get tags for a specific artwork
    /// </summary>
    /// <remarks>
    /// Retrieves all tags associated with the given artwork ID.
    /// </remarks>
    /// <param name="artworkId">The ID of the artwork</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of tags for the artwork</returns>
    /// <response code="200">Returns the artwork's tags</response>
    [HttpGet("/api/artworks/{artworkId}/tags")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<IReadOnlyCollection<TagResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByArtworkId(int artworkId, CancellationToken cancellationToken)
    {
        var result = await _tagService.GetByArtworkIdAsync(artworkId, cancellationToken);
        return Ok(result);
    }
}
