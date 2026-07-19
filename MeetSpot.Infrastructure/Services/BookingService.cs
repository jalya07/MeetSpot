using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Booking;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;
using QRCoder;

namespace MeetSpot.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly MeetSpotAppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IBookingNotificationService _notificationService;
    private readonly IMapper _mapper;

    public BookingService(MeetSpotAppDbContext context, IEmailService emailService, IBookingNotificationService notificationService, IMapper mapper)
    {
        _context = context;
        _emailService = emailService;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> CreateAsync(CreateBookingDto dto, int userId)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        Room? room = null;
        User? user = null;
        decimal totalPrice = 0;
        Booking? booking = null;

        try
        {
            var conflict = await _context.Bookings
                .AnyAsync(b => b.RoomId == dto.RoomId
                    && b.Status == BookingStatus.Active
                    && b.StartTime < dto.EndTime
                    && b.EndTime > dto.StartTime);

            if (conflict)
                throw new ConflictException("Room is already booked for this time slot");

            room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
                throw new NotFoundException("Room not found");

            user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var minDaysAhead = user.Type switch
            {
                UserType.Regular => 5,
                UserType.Corporate => 0,
                UserType.Premium => 0,
                _ => 5
            };

            if (dto.StartTime.Hour < 8 || dto.StartTime.Hour >= 18)
                throw new BadRequestException("Bookings are only available between 8:00 and 18:00");

            if (dto.EndTime.Hour > 18 || (dto.EndTime.Hour == 18 && dto.EndTime.Minute > 0))
                throw new BadRequestException("Bookings must end by 18:00");

            if (dto.StartTime < DateTime.UtcNow.AddDays(minDaysAhead))
                throw new BadRequestException($"Your account type requires booking at least {minDaysAhead} days in advance");

            var hours = (decimal)(dto.EndTime - dto.StartTime).TotalHours;
            var originalPrice = hours * room.PricePerHour;
            totalPrice = originalPrice;

            var now = DateTime.UtcNow;
            var activePromotion = await _context.Promotions
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .OrderByDescending(p => p.DiscountPercent)
                .FirstOrDefaultAsync();

            decimal discountAmount = 0;
            if (activePromotion != null)
            {
                discountAmount = totalPrice * (activePromotion.DiscountPercent / 100);
                totalPrice = totalPrice - discountAmount;
            }

            if (user.Role != UserRole.Admin && user.Type != UserType.Corporate)
            {
                if (user.Balance < totalPrice)
                    throw new BadRequestException("Insufficient balance");
            }

            var qrContent = $"MEETSPOT-BOOKING-{Guid.NewGuid()}";
            var qrCode = GenerateQrBase64(qrContent);

            booking = new Booking
            {
                Title = dto.Title,
                RoomId = dto.RoomId,
                UserId = userId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TotalPrice = totalPrice,
                OriginalPrice = activePromotion != null ? originalPrice : null,
                DiscountAmount = activePromotion != null ? discountAmount : null,
                PromotionId = activePromotion?.Id,
                Status = BookingStatus.Active,
                QrCode = qrCode
            };

            foreach (var email in dto.ParticipantEmails)
            {
                booking.Participants.Add(new BookingParticipant
                {
                    Email = email,
                    Name = email
                });
            }

            _context.Bookings.Add(booking);

            if (user.Role != UserRole.Admin && user.Type != UserType.Corporate)
            {
                user.Balance -= totalPrice;
                _context.Transactions.Add(new Transaction
                {
                    UserId = userId,
                    Amount = -totalPrice,
                    Type = TransactionType.BookingPayment,
                    Description = $"Booking: {dto.Title}",
                    BalanceAfter = user.Balance
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            throw new ConflictException("Room was booked by another request. Please try again.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        var bookingDetails = $"<p><b>Room:</b> {room!.Name}</p><p><b>Date:</b> {dto.StartTime:dd MMM yyyy}</p><p><b>Time:</b> {dto.StartTime:HH:mm} - {dto.EndTime:HH:mm}</p><p><b>Total:</b> ${totalPrice}</p>";
        await _emailService.SendBookingConfirmationAsync(user!.Email, $"{user.FirstName} {user.LastName}", bookingDetails);

        Console.WriteLine($"=== About to call NotifyBookingCreated ===");
        await _notificationService.NotifyBookingCreated(room!.Id, room.Name, dto.StartTime, dto.EndTime);
        Console.WriteLine($"=== NotifyBookingCreated called ===");

       var bookingWithDetails = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Include(b => b.Participants)
            .FirstOrDefaultAsync(b => b.Id == booking!.Id);

        return _mapper.Map<BookingResponseDto>(bookingWithDetails!);
    }

    public async Task<BookingResponseDto> UpdateAsync(int id, UpdateBookingDto dto, int userId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Participants)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null)
            throw new NotFoundException("Booking not found");

        if (booking.Status != BookingStatus.Active)
            throw new BadRequestException("Cannot update a cancelled or completed booking");

        if (dto.StartTime.HasValue) booking.StartTime = dto.StartTime.Value;
        if (dto.EndTime.HasValue) booking.EndTime = dto.EndTime.Value;
        if (dto.Title != null) booking.Title = dto.Title;
        booking.UpdatedAt = DateTime.UtcNow;

        if (dto.ParticipantEmails != null)
        {
            _context.BookingParticipants.RemoveRange(booking.Participants);
            foreach (var email in dto.ParticipantEmails)
                booking.Participants.Add(new BookingParticipant { Email = email, Name = email });
        }

        await _context.SaveChangesAsync();
        var updatedBooking = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Include(b => b.Participants)
            .FirstOrDefaultAsync(b => b.Id == id);

        return _mapper.Map<BookingResponseDto>(updatedBooking!);
    }

    public async Task CancelAsync(int id, int userId, bool isAdmin, string? reason)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            throw new NotFoundException("Booking not found");

        if (!isAdmin && booking.UserId != userId)
            throw new UnauthorizedException("Unauthorized");

        if (booking.Status != BookingStatus.Active)
            throw new BadRequestException("Booking is already cancelled or completed");

        var user = booking.User;
        var hoursUntilStart = (booking.StartTime - DateTime.UtcNow).TotalHours;

        decimal refundAmount = booking.TotalPrice;
        decimal penalty = 0;

        if (!isAdmin && user.Type != UserType.Corporate)
        {
            var freeHours = user.Type == UserType.Premium ? 1 : 4;
            var penaltyPercent = user.Type == UserType.Premium ? 0.25m : 0.50m;

            if (hoursUntilStart < freeHours)
            {
                penalty = booking.TotalPrice * penaltyPercent;
                refundAmount = booking.TotalPrice - penalty;
            }
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancelReason = reason;
        booking.UpdatedAt = DateTime.UtcNow;

        if (user.Type != UserType.Corporate && !isAdmin)
        {
            user.Balance += refundAmount;
            _context.Transactions.Add(new Transaction
            {
                UserId = userId,
                Amount = refundAmount,
                Type = TransactionType.Refund,
                Description = $"Refund for cancelled booking: {booking.Title}",
                BalanceAfter = user.Balance
            });
        }

        await _notificationService.NotifyBookingCancelled(booking.RoomId, booking.Room.Name);

        await _context.SaveChangesAsync();
    }

    public async Task<List<BookingResponseDto>> GetMyBookingsAsync(int userId)
    {
        var bookings = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Include(b => b.Participants)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();

        return bookings.Select(b => _mapper.Map<BookingResponseDto>(b)).ToList();
    }

    public async Task<List<BookingResponseDto>> GetAllAsync()
    {
        var bookings = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Include(b => b.Participants)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();

        return bookings.Select(b => _mapper.Map<BookingResponseDto>(b)).ToList();
    }

    public async Task<List<BookingResponseDto>> GetRoomCalendarAsync(int roomId, DateTime date)
    {
        var bookings = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Include(b => b.Participants)
            .Where(b => b.RoomId == roomId
                && b.Status == BookingStatus.Active
                && b.StartTime.Date == date.Date)
            .OrderBy(b => b.StartTime)
            .ToListAsync();

    return bookings.Select(b => _mapper.Map<BookingResponseDto>(b)).ToList();
    }

    public async Task CheckInAsync(int bookingId, int userId, bool isAdmin)
    {
        var booking = await _context.Bookings
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            throw new NotFoundException("Booking not found");

        if (!isAdmin && booking.UserId != userId)
            throw new UnauthorizedException("Unauthorized");

        if (booking.Status != BookingStatus.Active)
            throw new BadRequestException("Booking is not active");

        booking.IsCheckedIn = true;
        booking.CheckInTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private string GenerateQrBase64(string content)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var bytes = qrCode.GetGraphic(10);
        return Convert.ToBase64String(bytes);
    }


}