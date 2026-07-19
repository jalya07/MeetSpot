namespace MeetSpot.Core.Entities;

public class MentorRequest : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public string? LinkedInUrl { get; set; }
    public MentorRequestStatus Status { get; set; } = MentorRequestStatus.Pending;
}

public enum MentorRequestStatus
{
    Pending,
    Approved,
    Rejected
}