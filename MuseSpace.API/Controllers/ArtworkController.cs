using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Results;
using MuseSpace.Core.Enums;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Endpoints for managing artworks, including uploading media and handling artwork metadata.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ArtworkController : ControllerBase
{
    private readonly IArtworkService _artworkService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtworkController"/> class.
    /// </summary>
    /// <param name="artworkService">The artwork service.</param>
    public ArtworkController(IArtworkService artworkService)
    {
        _artworkService = artworkService;
    }

    /// <summary>
    /// Create a new artwork
    /// </summary>
    /// <remarks>
    /// Uploads an artwork media file (image/video) and saves the metadata and tags.
    /// Requires authentication.
    /// </remarks>
    /// <param name="request">Artwork creation details including the media file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created artwork response</returns>
    /// <response code="200">Artwork created successfully</response>
    /// <response code="400">Creation failed - invalid data or media upload failure</response>
    /// <response code="401">Unauthorized - user not authenticated</response>
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(GenericResult<ArtworkResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromForm] CreateArtworkRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _artworkService.CreateAsync(userId, request, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Get an artwork by ID
    /// </summary>
    /// <remarks>
    /// Retrieves full details of a specific artwork. Also increments its view count.
    /// </remarks>
    /// <param name="id">The ID of the artwork</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The artwork details</returns>
    /// <response code="200">Returns the artwork details</response>
    /// <response code="404">Artwork not found</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<ArtworkResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? userId = string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim);

        var result = await _artworkService.GetByIdAsync(id, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get artworks by a specific user
    /// </summary>
    /// <remarks>
    /// Retrieves a paginated list of artworks created by the specified user.
    /// </remarks>
    /// <param name="userId">The ID of the creator user</param>
    /// <param name="page">The page number for pagination</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of artworks</returns>
    /// <response code="200">Returns the list of artworks</response>
    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<ArtworkResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? currentUserId = string.IsNullOrEmpty(currentUserIdClaim) ? null : int.Parse(currentUserIdClaim);

        var result = await _artworkService.GetByCreatorIdAsync(userId, page, pageSize, currentUserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get artworks liked by the current user
    /// </summary>
    [HttpGet("liked")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<PagedResult<ArtworkResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLikedArtworks([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _artworkService.GetLikedArtworksAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get artworks bookmarked by the current user
    /// </summary>
    [HttpGet("bookmarked")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<PagedResult<ArtworkResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookmarkedArtworks([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _artworkService.GetBookmarkedArtworksAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update an existing artwork
    /// </summary>
    /// <remarks>
    /// Updates the metadata (title, description) of an existing artwork.
    /// Only the creator of the artwork can update it. Requires authentication.
    /// </remarks>
    /// <param name="id">The ID of the artwork to update</param>
    /// <param name="request">The updated metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated artwork response</returns>
    /// <response code="200">Artwork updated successfully</response>
    /// <response code="400">Bad request - invalid input</response>
    /// <response code="403">Forbidden - user is not the creator</response>
    /// <response code="404">Artwork not found</response>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<ArtworkResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateArtworkRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _artworkService.UpdateAsync(id, userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete an artwork
    /// </summary>
    /// <remarks>
    /// Soft-deletes an artwork. Only the creator of the artwork can delete it.
    /// Requires authentication.
    /// </remarks>
    /// <param name="id">The ID of the artwork to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A boolean indicating success</returns>
    /// <response code="200">Artwork deleted successfully</response>
    /// <response code="400">Bad request</response>
    /// <response code="403">Forbidden - user is not the creator</response>
    /// <response code="404">Artwork not found</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _artworkService.DeleteAsync(id, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }

        return Ok(result);
    }
}
