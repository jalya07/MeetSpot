using System;
namespace MeetSpot.Core.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public UserType Type { get; set; }
        public string? CompanyDomain { get; set; }
        public decimal Balance { get; set; } = 0;
        public bool IsEmailConfirmed { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public Subscription? Subscription { get; set; }
    }
    public enum UserRole
    {
        Admin,
        User
    }

    public enum UserType
    {
        Regular,
        Corporate,
        Premium
    }
}
