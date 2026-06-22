namespace MeetSpot.Core.DTOs.Contact;

public class CreateContactMessageDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "General"; // General, CorporateContract, Support
}