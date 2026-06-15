namespace MeetSpot.Core.Entities;

public class Subscription : BaseEntity
{
    public SubscriptionPlan Plan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal Price { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

public enum SubscriptionPlan
{
    Monthly,
    Annual
}