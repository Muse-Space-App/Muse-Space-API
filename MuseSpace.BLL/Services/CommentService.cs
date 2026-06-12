using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;
using Microsoft.EntityFrameworkCore;

namespace MuseSpace.BLL.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IArtworkRepository _artworkRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public CommentService(ICommentRepository commentRepository, IArtworkRepository artworkRepository, IUserRepository userRepository, IMapper mapper, INotificationService notificationService)
    {
        _commentRepository = commentRepository;
        _artworkRepository = artworkRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<GenericResult<CommentResponse>> CreateCommentAsync(int artworkId, int userId, CreateCommentRequest request, CancellationToken cancellationToken = default)
    {
        var artwork = await _artworkRepository.GetByIdAsync(artworkId, cancellationToken);
        if (artwork == null || artwork.IsSoftDeleted)
        {
            return GenericResult<CommentResponse>.Failure("Artwork not found", ErrorType.NotFound);
        }

        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _commentRepository.GetByIdAsync(request.ParentCommentId.Value, cancellationToken);
            if (parentComment == null || parentComment.IsSoftDeleted || parentComment.ArtworkId != artworkId)
            {
                return GenericResult<CommentResponse>.Failure("Parent comment not found or invalid", ErrorType.ValidationFailed);
            }
        }

        var comment = new Comment
        {
            ArtworkId = artworkId,
            UserId = userId,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment, cancellationToken);

        // Trigger Notification
        if (artwork.CreatorId != userId) // Don't notify self
        {
            await _notificationService.CreateNotificationAsync(
                artwork.CreatorId,
                "Comment",
                "Someone commented on your artwork.",
                $"/artwork/{artworkId}",
                userId,
                artworkId,
                cancellationToken
            );
        }

        // Fetch the user to populate the response properly
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        
        // Let's use GetByUsernameAsync to get full profile
        if (user != null)
        {
            user = await _userRepository.GetByUsernameAsync(user.Username, cancellationToken);
        }

        comment.User = user;

        var response = _mapper.Map<CommentResponse>(comment);
        return GenericResult<CommentResponse>.Success(response, "Comment created successfully");
    }

    public async Task<GenericResult<bool>> DeleteCommentAsync(int commentId, int userId, CancellationToken cancellationToken = default)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);
        if (comment == null || comment.IsSoftDeleted)
        {
            return GenericResult<bool>.Failure("Comment not found", ErrorType.NotFound);
        }

        if (comment.UserId != userId)
        {
            return GenericResult<bool>.Failure("Only the author can delete this comment", ErrorType.Unauthorized);
        }

        comment.IsSoftDeleted = true;
        await _commentRepository.UpdateAsync(comment, cancellationToken);

        return GenericResult<bool>.Success(true, "Comment deleted successfully");
    }

    public async Task<GenericResult<PagedResult<CommentResponse>>> GetCommentsByArtworkIdAsync(int artworkId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var comments = await _commentRepository.GetByArtworkIdAsync(artworkId, page, pageSize, cancellationToken);
        var totalCount = await _commentRepository.CountByArtworkIdAsync(artworkId, cancellationToken);

        var responses = _mapper.Map<IReadOnlyCollection<CommentResponse>>(comments);

        var pagedResult = new PagedResult<CommentResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return GenericResult<PagedResult<CommentResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<PagedResult<CommentResponse>>> GetRepliesAsync(int commentId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var replies = await _commentRepository.GetRepliesAsync(commentId, page, pageSize, cancellationToken);

        // Accurate count of replies would require a dedicated repository method
        // Using arbitrary count for now to signify more pages if full
        var totalCount = replies.Count == pageSize ? page * pageSize + 1 : (page - 1) * pageSize + replies.Count;

        var responses = _mapper.Map<IReadOnlyCollection<CommentResponse>>(replies);

        var pagedResult = new PagedResult<CommentResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return GenericResult<PagedResult<CommentResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<CommentResponse>> UpdateCommentAsync(int commentId, int userId, UpdateCommentRequest request, CancellationToken cancellationToken = default)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);
        if (comment == null || comment.IsSoftDeleted)
        {
            return GenericResult<CommentResponse>.Failure("Comment not found", ErrorType.NotFound);
        }

        if (comment.UserId != userId)
        {
            return GenericResult<CommentResponse>.Failure("Only the author can update this comment", ErrorType.Unauthorized);
        }

        comment.Content = request.Content;
        comment.IsEdited = true;
        comment.EditedAtUtc = DateTime.UtcNow;

        await _commentRepository.UpdateAsync(comment, cancellationToken);

        var response = _mapper.Map<CommentResponse>(comment);
        return GenericResult<CommentResponse>.Success(response, "Comment updated successfully");
    }
}
