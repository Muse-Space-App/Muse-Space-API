using MuseSpace.BLL.DTO;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Interfaces.Services;

public interface ICommentService
{
    Task<GenericResult<CommentResponse>> CreateCommentAsync(int artworkId, int userId, CreateCommentRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<CommentResponse>>> GetCommentsByArtworkIdAsync(int artworkId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<PagedResult<CommentResponse>>> GetRepliesAsync(int commentId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GenericResult<CommentResponse>> UpdateCommentAsync(int commentId, int userId, UpdateCommentRequest request, CancellationToken cancellationToken = default);
    Task<GenericResult<bool>> DeleteCommentAsync(int commentId, int userId, CancellationToken cancellationToken = default);
}
