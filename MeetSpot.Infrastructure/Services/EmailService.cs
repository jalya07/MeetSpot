using MailKit.Net.Smtp;
using MailKit.Security;
using MeetSpot.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace MeetSpot.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["Email:SenderName"],
            _configuration["Email:SenderEmail"]));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _configuration["Email:Host"],
            int.Parse(_configuration["Email:Port"]!),
            SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(
            _configuration["Email:SenderEmail"],
            _configuration["Email:Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendEmailConfirmationAsync(string toEmail, string fullName, string confirmationLink)
    {
        var html = $"""
            <h2>Welcome to MeetSpot, {fullName}!</h2>
            <p>Please confirm your email by clicking the link below:</p>
            <a href="{confirmationLink}" style="background:#4F46E5;color:white;padding:12px 24px;border-radius:6px;text-decoration:none;">
                Confirm Email
            </a>
            <p>This link expires in 24 hours.</p>
        """;

        await SendAsync(toEmail, fullName, "Confirm your MeetSpot account", html);
    }

    public async Task SendPasswordResetAsync(string toEmail, string fullName, string resetLink)
    {
        var html = $"""
            <h2>Reset your password, {fullName}</h2>
            <p>Click the link below to reset your password:</p>
            <a href="{resetLink}" style="background:#DC2626;color:white;padding:12px 24px;border-radius:6px;text-decoration:none;">
                Reset Password
            </a>
            <p>This link expires in 1 hour. If you didn't request this, ignore this email.</p>
        """;

        await SendAsync(toEmail, fullName, "Reset your MeetSpot password", html);
    }

    public async Task SendBookingConfirmationAsync(string toEmail, string fullName, string bookingDetails)
    {
        var html = $"""
            <h2>Booking Confirmed, {fullName}!</h2>
            <p>Your meeting room has been successfully booked.</p>
            <div style="background:#F3F4F6;padding:16px;border-radius:8px;">
                {bookingDetails}
            </div>
            <p>You can view your booking in your dashboard.</p>
        """;

        await SendAsync(toEmail, fullName, "MeetSpot Booking Confirmation", html);
    }
}