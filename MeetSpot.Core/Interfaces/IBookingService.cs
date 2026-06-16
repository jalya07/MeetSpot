using MeetSpot.Core.DTOs.Booking;

namespace MeetSpot.Core.Interfaces;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(CreateBookingDto dto, int userId);
    Task<BookingResponseDto> UpdateAsync(int id, UpdateBookingDto dto, int userId);
    Task CancelAsync(int id, int userId, string? reason);
    Task<List<BookingResponseDto>> GetMyBookingsAsync(int userId);
    Task<List<BookingResponseDto>> GetAllAsync();
    Task<List<BookingResponseDto>> GetRoomCalendarAsync(int roomId, DateTime date);
    Task CheckInAsync(int bookingId);
}