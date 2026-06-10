using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Services;

public class InteractionService : IInteractionService
{
    private readonly IInteractionRepository _interactionRepository;
    private readonly IArtworkRepository _artworkRepository;
    private readonly INotificationService _notificationService;

    public InteractionService(
        IInteractionRepository interactionRepository, 
        IArtworkRepository artworkRepository,
        INotificationService notificationService)
    {
        _interactionRepository = interactionRepository;
        _artworkRepository = artworkRepository;
        _notificationService = notificationService;
    }

    public async Task<GenericResult<bool>> ToggleLikeAsync(int artworkId, int userId, CancellationToken cancellationToken = default)
    {
        var artwork = await _artworkRepository.GetByIdAsync(artworkId, cancellationToken);
        if (artwork == null || artwork.IsSoftDeleted)
        {
            return GenericResult<bool>.Failure("Artwork not found", ErrorType.NotFound);
        }

        var hasLiked = await _interactionRepository.HasUserLikedAsync(artworkId, userId, cancellationToken);

        if (hasLiked)
        {
            await _interactionRepository.RemoveLikeAsync(new Like { ArtworkId = artworkId, UserId = userId }, cancellationToken);
            return GenericResult<bool>.Success(false, "Artwork unliked successfully");
        }
        else
        {
            await _interactionRepository.AddLikeAsync(new Like { ArtworkId = artworkId, UserId = userId }, cancellationToken);
            
            // Trigger Notification
            if (artwork.CreatorId != userId) // Don't notify self
            {
                await _notificationService.CreateNotificationAsync(
                    artwork.CreatorId,
                    "Like",
                    "Someone liked your artwork.",
                    $"/artwork/{artworkId}",
                    userId,
                    artworkId,
                    cancellationToken
                );
            }

            return GenericResult<bool>.Success(true, "Artwork liked successfully");
        }
    }

    public async Task<GenericResult<bool>> ToggleBookmarkAsync(int artworkId, int userId, CancellationToken cancellationToken = default)
    {
        var artwork = await _artworkRepository.GetByIdAsync(artworkId, cancellationToken);
        if (artwork == null || artwork.IsSoftDeleted)
        {
            return GenericResult<bool>.Failure("Artwork not found", ErrorType.NotFound);
        }

        var hasBookmarked = await _interactionRepository.HasUserBookmarkedAsync(artworkId, userId, cancellationToken);

        if (hasBookmarked)
        {
            await _interactionRepository.RemoveBookmarkAsync(new Bookmark { ArtworkId = artworkId, UserId = userId }, cancellationToken);
            return GenericResult<bool>.Success(false, "Artwork removed from bookmarks");
        }
        else
        {
            await _interactionRepository.AddBookmarkAsync(new Bookmark { ArtworkId = artworkId, UserId = userId }, cancellationToken);
            return GenericResult<bool>.Success(true, "Artwork bookmarked successfully");
        }
    }

    public async Task<GenericResult<bool>> RecordShareAsync(int artworkId, int userId, string platform, CancellationToken cancellationToken = default)
    {
        var artwork = await _artworkRepository.GetByIdAsync(artworkId, cancellationToken);
        if (artwork == null || artwork.IsSoftDeleted)
        {
            return GenericResult<bool>.Failure("Artwork not found", ErrorType.NotFound);
        }

        var share = new Share
        {
            ArtworkId = artworkId,
            UserId = userId,
            Platform = platform
        };

        await _interactionRepository.AddShareAsync(share, cancellationToken);
        return GenericResult<bool>.Success(true, "Share recorded successfully");
    }
}
