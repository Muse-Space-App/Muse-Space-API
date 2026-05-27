using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Results;
using System.Security.Claims;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Endpoints for managing comments on artworks.
/// </summary>
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentController"/> class.
    /// </summary>
    /// <param name="commentService">The comment service.</param>
    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    /// <summary>
    /// Create a new comment or reply
    /// </summary>
    /// <param name="artworkId">The ID of the artwork</param>
    /// <param name="request">The comment content and optional parent ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("api/artworks/{artworkId}/comments")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<CommentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateComment(int artworkId, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commentService.CreateCommentAsync(artworkId, userId, request, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Get comments for an artwork
    /// </summary>
    /// <param name="artworkId">The ID of the artwork</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("api/artworks/{artworkId}/comments")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<CommentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComments(int artworkId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _commentService.GetCommentsByArtworkIdAsync(artworkId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get replies for a specific comment
    /// </summary>
    /// <param name="commentId">The ID of the parent comment</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("api/comments/{commentId}/replies")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResult<PagedResult<CommentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReplies(int commentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _commentService.GetRepliesAsync(commentId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update an existing comment
    /// </summary>
    /// <param name="commentId">The ID of the comment</param>
    /// <param name="request">The updated comment content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("api/comments/{commentId}")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<CommentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commentService.UpdateCommentAsync(commentId, userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    /// <param name="commentId">The ID of the comment</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("api/comments/{commentId}")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteComment(int commentId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commentService.DeleteCommentAsync(commentId, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == ErrorType.NotFound) return NotFound(result);
            if (result.ErrorType == ErrorType.Unauthorized) return Forbid();
            return BadRequest(result);
        }

        return Ok(result);
    }
}
