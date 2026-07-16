namespace MeetSpot.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class Promotion : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercent { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public PromotionType Type { get; set; } = PromotionType.General;
}

public enum PromotionType
{
    General,
    EarlyBird,
    OffPeak,
    Corporate
}