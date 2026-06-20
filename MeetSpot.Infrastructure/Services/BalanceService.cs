using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Balance;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.Infrastructure.Services;

public class BalanceService : IBalanceService
{
    private readonly MeetSpotAppDbContext _context;
    private readonly IMapper _mapper;

    public BalanceService(MeetSpotAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TransactionResponseDto> TopUpAsync(int userId, TopUpBalanceDto dto)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        if (dto.Amount <= 0)
            throw new BadRequestException("Amount must be greater than 0");

        // Fake Stripe — просто имитация, без реальной обработки карты
        user.Balance += dto.Amount;

        var transaction = new Transaction
        {
            UserId = userId,
            Amount = dto.Amount,
            Type = TransactionType.Deposit,
            Description = $"Balance top-up via card ending in {dto.CardNumber[^4..]}",
            BalanceAfter = user.Balance
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return _mapper.Map<TransactionResponseDto>(transaction);
    }

    public async Task<List<TransactionResponseDto>> GetTransactionHistoryAsync(int userId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return transactions.Select(t => _mapper.Map<TransactionResponseDto>(t)).ToList();
    }

    public async Task<decimal> GetBalanceAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        return user.Balance;
    }
}