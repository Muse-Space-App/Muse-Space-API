using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Results;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Handles unified searching across artworks, users, and tags.
/// </summary>
[ApiController]
[Route("api/search")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchController"/> class.
    /// </summary>
    /// <param name="searchService">The search service.</param>
    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    /// <summary>
    /// Searches for artworks, users, and tags matching the query.
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unified search results</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<SearchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? query = "", [FromQuery] string? type = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _searchService.SearchAsync(query, type, page, pageSize, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Advanced Pixiv-like search for artworks with exact tags and custom sorting.
    /// </summary>
    /// <param name="request">Search parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged list of matching artworks</returns>
    [HttpGet("advanced")]
    [ProducesResponseType(typeof(GenericResult<PagedResult<ArtworkResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AdvancedSearch([FromQuery] AdvancedSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await _searchService.AdvancedSearchAsync(request, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
