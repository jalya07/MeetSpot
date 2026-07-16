namespace MeetSpot.Core.Entities;

public class CompanyRequest : BaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? LogoUrl { get; set; }
    public string? Message { get; set; }
    public CompanyRequestStatus Status { get; set; } = CompanyRequestStatus.Pending;
}

public enum CompanyRequestStatus
{
    Pending,
    Approved,
    Rejected
}