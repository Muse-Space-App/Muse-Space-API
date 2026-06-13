using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Enums;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Core.Results;

namespace MuseSpace.BLL.Services;

public class CommissionService : ICommissionService
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public CommissionService(
        ICommissionRepository commissionRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        IMapper mapper)
    {
        _commissionRepository = commissionRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<GenericResult<CommissionResponse>> CreateCommissionAsync(int requesterId, CreateCommissionRequest request, CancellationToken cancellationToken = default)
    {
        if (requesterId == request.ArtistId)
            return GenericResult<CommissionResponse>.Failure("Cannot request a commission from yourself", ErrorType.ValidationFailed);

        var artist = await _userRepository.GetByIdAsync(request.ArtistId, cancellationToken);
        if (artist == null)
            return GenericResult<CommissionResponse>.Failure("Artist not found", ErrorType.NotFound);

        var commission = new Commission
        {
            RequesterId = requesterId,
            ArtistId = request.ArtistId,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            DeadlineUtc = request.DeadlineUtc,
            Status = CommissionStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _commissionRepository.AddAsync(commission, cancellationToken);

        // Notify Artist
        await _notificationService.CreateNotificationAsync(
            userId: request.ArtistId,
            type: "CommissionRequest",
            message: $"You received a new commission request: {request.Title}",
            actionUrl: $"/commissions/{commission.Id}",
            relatedUserId: requesterId,
            cancellationToken: cancellationToken);

        var response = await GetCommissionAsync(commission.Id, requesterId, cancellationToken);
        return response;
    }

    public async Task<GenericResult<PagedResult<CommissionResponse>>> GetCommissionsByRequesterAsync(int requesterId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var commissions = await _commissionRepository.GetCommissionsByRequesterAsync(requesterId, (page - 1) * pageSize, pageSize, cancellationToken);

        var responses = commissions.Select(c => MapToResponse(c)).ToList();

        var pagedResult = new PagedResult<CommissionResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 0 // Missing count logic in repo for brevity
        };

        return GenericResult<PagedResult<CommissionResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<PagedResult<CommissionResponse>>> GetCommissionsByArtistAsync(int artistId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var commissions = await _commissionRepository.GetCommissionsByArtistAsync(artistId, (page - 1) * pageSize, pageSize, cancellationToken);

        var responses = commissions.Select(c => MapToResponse(c)).ToList();

        var pagedResult = new PagedResult<CommissionResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 0
        };

        return GenericResult<PagedResult<CommissionResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<CommissionResponse>> GetCommissionAsync(int commissionId, int userId, CancellationToken cancellationToken = default)
    {
        var commission = await _commissionRepository.GetCommissionWithDetailsAsync(commissionId, cancellationToken);
        if (commission == null)
            return GenericResult<CommissionResponse>.Failure("Commission not found", ErrorType.NotFound);

        if (commission.RequesterId != userId && commission.ArtistId != userId)
            return GenericResult<CommissionResponse>.Failure("Unauthorized", ErrorType.Unauthorized);

        var response = MapToResponse(commission);
        return GenericResult<CommissionResponse>.Success(response);
    }

    public async Task<GenericResult<CommissionResponse>> UpdateCommissionStatusAsync(int commissionId, int userId, UpdateCommissionStatusRequest request, CancellationToken cancellationToken = default)
    {
        var commission = await _commissionRepository.GetCommissionWithDetailsAsync(commissionId, cancellationToken);
        if (commission == null)
            return GenericResult<CommissionResponse>.Failure("Commission not found", ErrorType.NotFound);

        if (commission.RequesterId != userId && commission.ArtistId != userId)
            return GenericResult<CommissionResponse>.Failure("Unauthorized", ErrorType.Unauthorized);

        // Simple validation: only artist can accept/reject/complete. Requester can cancel.
        if (request.Status == CommissionStatus.Cancelled && commission.RequesterId != userId)
            return GenericResult<CommissionResponse>.Failure("Only requester can cancel", ErrorType.Forbidden);

        if ((request.Status == CommissionStatus.Accepted ||
             request.Status == CommissionStatus.Rejected ||
             request.Status == CommissionStatus.InProgress ||
             request.Status == CommissionStatus.Completed) && commission.ArtistId != userId)
            return GenericResult<CommissionResponse>.Failure("Only artist can perform this action", ErrorType.Forbidden);

        commission.Status = request.Status;
        if (request.Status == CommissionStatus.Completed)
            commission.CompletedAtUtc = DateTime.UtcNow;

        await _commissionRepository.UpdateAsync(commission, cancellationToken);

        // Notify the other party
        int notifyUserId = commission.RequesterId == userId ? commission.ArtistId : commission.RequesterId;
        await _notificationService.CreateNotificationAsync(
            userId: notifyUserId,
            type: "CommissionStatusUpdate",
            message: $"Commission '{commission.Title}' status updated to {request.Status}",
            actionUrl: $"/commissions/{commission.Id}",
            relatedUserId: userId,
            cancellationToken: cancellationToken);

        return GenericResult<CommissionResponse>.Success(MapToResponse(commission));
    }

    public async Task<GenericResult<PagedResult<CommissionMessageResponse>>> GetCommissionMessagesAsync(int commissionId, int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var commission = await _commissionRepository.GetByIdAsync(commissionId, cancellationToken);
        if (commission == null)
            return GenericResult<PagedResult<CommissionMessageResponse>>.Failure("Commission not found", ErrorType.NotFound);

        if (commission.RequesterId != userId && commission.ArtistId != userId)
            return GenericResult<PagedResult<CommissionMessageResponse>>.Failure("Unauthorized", ErrorType.Unauthorized);

        await _commissionRepository.MarkMessagesAsReadAsync(commissionId, userId, cancellationToken);

        var messages = await _commissionRepository.GetCommissionMessagesAsync(commissionId, (page - 1) * pageSize, pageSize, cancellationToken);

        var responses = messages.Select(m => new CommissionMessageResponse
        {
            Id = m.Id,
            CommissionId = m.CommissionId,
            SenderId = m.SenderId,
            SenderUsername = m.Sender?.Username ?? "Unknown",
            SenderAvatarUrl = m.Sender?.UserProfile?.AvatarUrl,
            Content = m.Content,
            AttachmentUrl = m.AttachmentUrl,
            AttachmentType = m.AttachmentType,
            IsRead = m.IsRead,
            CreatedAtUtc = m.CreatedAtUtc
        }).ToList();

        var pagedResult = new PagedResult<CommissionMessageResponse>
        {
            Items = responses,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 0
        };

        return GenericResult<PagedResult<CommissionMessageResponse>>.Success(pagedResult);
    }

    public async Task<GenericResult<CommissionMessageResponse>> SendMessageAsync(int commissionId, int senderId, CreateCommissionMessageRequest request, CancellationToken cancellationToken = default)
    {
        var commission = await _commissionRepository.GetByIdAsync(commissionId, cancellationToken);
        if (commission == null)
            return GenericResult<CommissionMessageResponse>.Failure("Commission not found", ErrorType.NotFound);

        if (commission.RequesterId != senderId && commission.ArtistId != senderId)
            return GenericResult<CommissionMessageResponse>.Failure("Unauthorized", ErrorType.Unauthorized);

        var message = new CommissionMessage
        {
            CommissionId = commissionId,
            SenderId = senderId,
            Content = request.Content,
            AttachmentUrl = request.AttachmentUrl,
            AttachmentType = request.AttachmentType,
            IsRead = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _commissionRepository.AddMessageAsync(message, cancellationToken);

        var sender = await _userRepository.GetByIdAsync(senderId, cancellationToken);

        var response = new CommissionMessageResponse
        {
            Id = message.Id,
            CommissionId = commissionId,
            SenderId = senderId,
            SenderUsername = sender?.Username ?? "Unknown",
            SenderAvatarUrl = sender?.UserProfile?.AvatarUrl,
            Content = message.Content,
            AttachmentUrl = message.AttachmentUrl,
            AttachmentType = message.AttachmentType,
            IsRead = message.IsRead,
            CreatedAtUtc = message.CreatedAtUtc
        };

        // Notify the other party
        int notifyUserId = commission.RequesterId == senderId ? commission.ArtistId : commission.RequesterId;
        await _notificationService.CreateNotificationAsync(
            userId: notifyUserId,
            type: "CommissionMessage",
            message: $"New message on commission '{commission.Title}'",
            actionUrl: $"/commissions/{commission.Id}",
            relatedUserId: senderId,
            cancellationToken: cancellationToken);

        return GenericResult<CommissionMessageResponse>.Success(response);
    }

    private CommissionResponse MapToResponse(Commission commission)
    {
        return new CommissionResponse
        {
            Id = commission.Id,
            RequesterId = commission.RequesterId,
            RequesterUsername = commission.Requester?.Username ?? "Unknown",
            RequesterAvatarUrl = commission.Requester?.UserProfile?.AvatarUrl,
            ArtistId = commission.ArtistId,
            ArtistUsername = commission.Artist?.Username ?? "Unknown",
            ArtistAvatarUrl = commission.Artist?.UserProfile?.AvatarUrl,
            Title = commission.Title,
            Description = commission.Description,
            Price = commission.Price,
            Status = commission.Status,
            DeadlineUtc = commission.DeadlineUtc,
            CompletedAtUtc = commission.CompletedAtUtc,
            CreatedAtUtc = commission.CreatedAtUtc
        };
    }
}
