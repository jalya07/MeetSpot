namespace MeetSpot.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(string toEmail, string fullName, string confirmationLink);
    Task SendPasswordResetAsync(string toEmail, string fullName, string resetLink);
    Task SendBookingConfirmationAsync(string toEmail, string fullName, string bookingDetails);
    Task SendTwoFactorCodeAsync(string toEmail, string fullName, string code);
}