using AutoMapper;
using MeetSpot.Core.DTOs.Auth;
using MeetSpot.Core.DTOs.Balance;
using MeetSpot.Core.DTOs.Blog;
using MeetSpot.Core.DTOs.Booking;
using MeetSpot.Core.DTOs.Company;
using MeetSpot.Core.DTOs.Contact;
using MeetSpot.Core.DTOs.Room;
using MeetSpot.Core.DTOs.Subscription;
using MeetSpot.Core.DTOs.User;
using MeetSpot.Core.Entities;

namespace MeetSpot.Core.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => 
                Enum.Parse<UserType>(src.UserType, true)));

        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => 
                $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => 
                src.Role.ToString()))
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => 
                src.Type.ToString()))
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());
        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));
        CreateMap<User, UserListItemDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => 
                $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.Type.ToString()));

        // Room mappings
        CreateMap<CreateRoomDto, Room>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => 
                Enum.Parse<RoomType>(src.Type, true)))
            .ForMember(dest => dest.Equipment, opt => opt.Ignore());

        CreateMap<Room, RoomResponseDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Equipment, opt => opt.MapFrom(src => src.Equipment.Select(e => e.Name).ToList()))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                src.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()));

        // Booking mappings
        CreateMap<Booking, BookingResponseDto>()
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => 
                src.Room.Name))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => 
                $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 
                src.Status.ToString()))
            .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => 
                src.Participants.Select(p => p.Email).ToList()));
        //subscription mappings
        CreateMap<Subscription, SubscriptionResponseDto>()
            .ForMember(dest => dest.Plan, opt => opt.MapFrom(src => src.Plan.ToString()));

        CreateMap<Transaction, TransactionResponseDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        //company mappings
        CreateMap<CreateCompanyDto, Company>();
        CreateMap<Company, CompanyResponseDto>();

        CreateMap<CreateBlogPostDto, BlogPost>();
        CreateMap<BlogPost, BlogPostResponseDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => 
                $"{src.Author.FirstName} {src.Author.LastName}"));

        CreateMap<CreateContactMessageDto, ContactMessage>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => 
                Enum.Parse<MessageType>(src.Type, true)));

        CreateMap<ContactMessage, ContactMessageResponseDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}