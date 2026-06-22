using MeetSpot.Core.DTOs.Contact;

namespace MeetSpot.Core.Interfaces;

public interface IContactService
{
    Task<ContactMessageResponseDto> CreateAsync(CreateContactMessageDto dto);
    Task<List<ContactMessageResponseDto>> GetAllAsync();
    Task MarkAsReadAsync(int id);
}