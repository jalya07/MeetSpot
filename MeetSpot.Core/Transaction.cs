namespace MeetSpot.Core.Entities;

public class Transaction : BaseEntity
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? Description { get; set; }
    public decimal BalanceAfter { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int? BookingId { get; set; }
    public Booking? Booking { get; set; }
}

public enum TransactionType
{
    Deposit,
    BookingPayment,
    CancellationFee,
    Refund,
    SubscriptionPayment
}