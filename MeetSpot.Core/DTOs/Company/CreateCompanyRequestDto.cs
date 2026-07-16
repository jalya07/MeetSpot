namespace MeetSpot.Core.DTOs.Company;

public class CreateCompanyRequestDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? LogoUrl { get; set; }
    public string? Message { get; set; }
}