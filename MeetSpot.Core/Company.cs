namespace MeetSpot.Core.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Industry { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
}