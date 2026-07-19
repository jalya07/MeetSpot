namespace MeetSpot.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;


public class Mentor : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Skills { get; set; } 
    [Column(TypeName = "decimal(3,1)")]
    public decimal Rating { get; set; } = 5;
    public int SessionsCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}