using MeetSpot.Core.DTOs.Balance;

namespace MeetSpot.Core.Interfaces;

public interface IBalanceService
{
    Task<TransactionResponseDto> TopUpAsync(int userId, TopUpBalanceDto dto);
    Task<List<TransactionResponseDto>> GetTransactionHistoryAsync(int userId);
    Task<decimal> GetBalanceAsync(int userId);
}