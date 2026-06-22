using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Contact;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.Infrastructure.Services;

public class ContactService : IContactService
{
    private readonly MeetSpotAppDbContext _context;
    private readonly IMapper _mapper;

    public ContactService(MeetSpotAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ContactMessageResponseDto> CreateAsync(CreateContactMessageDto dto)
    {
        var message = _mapper.Map<ContactMessage>(dto);

        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync();

        return _mapper.Map<ContactMessageResponseDto>(message);
    }

    public async Task<List<ContactMessageResponseDto>> GetAllAsync()
    {
        var messages = await _context.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return messages.Select(m => _mapper.Map<ContactMessageResponseDto>(m)).ToList();
    }

    public async Task MarkAsReadAsync(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);

        if (message == null)
            throw new NotFoundException("Message not found");

        message.IsRead = true;
        message.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}