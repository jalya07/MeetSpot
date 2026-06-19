using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Subscription;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly MeetSpotAppDbContext _context;
    private readonly IMapper _mapper;

    private const decimal MonthlyPrice = 19.99m;
    private const decimal AnnualPrice = 199.99m;

    public SubscriptionService(MeetSpotAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SubscriptionResponseDto> SubscribeAsync(int userId, CreateSubscriptionDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        if (user.Type == UserType.Corporate)
            throw new BadRequestException("Corporate accounts already have premium benefits");

        if (user.Subscription != null && user.Subscription.IsActive)
            throw new ConflictException("You already have an active subscription");

        var plan = Enum.Parse<SubscriptionPlan>(dto.Plan, true);
        var price = plan == SubscriptionPlan.Monthly ? MonthlyPrice : AnnualPrice;
        var duration = plan == SubscriptionPlan.Monthly ? TimeSpan.FromDays(30) : TimeSpan.FromDays(365);

        if (user.Balance < price)
            throw new BadRequestException("Insufficient balance to subscribe");

        var subscription = new Subscription
        {
            UserId = userId,
            Plan = plan,
            Price = price,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.Add(duration),
            IsActive = true
        };

        _context.Subscriptions.Add(subscription);

        user.Balance -= price;
        user.Type = UserType.Premium;

        _context.Transactions.Add(new Transaction
        {
            UserId = userId,
            Amount = -price,
            Type = TransactionType.SubscriptionPayment,
            Description = $"{plan} subscription",
            BalanceAfter = user.Balance
        });

        await _context.SaveChangesAsync();

        return _mapper.Map<SubscriptionResponseDto>(subscription);
    }

    public async Task<SubscriptionResponseDto?> GetMyActiveSubscriptionAsync(int userId)
    {
        var subscription = await _context.Subscriptions
            .Where(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync();

        return subscription == null ? null : _mapper.Map<SubscriptionResponseDto>(subscription);
    }

    public async Task CancelAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Subscription == null || !user.Subscription.IsActive)
            throw new NotFoundException("No active subscription found");

        user.Subscription.IsActive = false;
        user.Type = UserType.Regular;

        await _context.SaveChangesAsync();
    }
}