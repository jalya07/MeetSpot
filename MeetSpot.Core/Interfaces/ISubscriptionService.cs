using MeetSpot.Core.DTOs.Subscription;

namespace MeetSpot.Core.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionResponseDto> SubscribeAsync(int userId, CreateSubscriptionDto dto);
    Task<SubscriptionResponseDto?> GetMyActiveSubscriptionAsync(int userId);
    Task CancelAsync(int userId);
}