using FluentValidation;
using MeetSpot.Core.DTOs.Room;

namespace MeetSpot.Core.Validators;

public class UpdateRoomValidator : AbstractValidator<UpdateRoomDto>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.Floor)
            .GreaterThan(0).When(x => x.Floor.HasValue)
            .WithMessage("Floor must be greater than 0");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).When(x => x.Capacity.HasValue)
            .WithMessage("Capacity must be greater than 0");

        RuleFor(x => x.PricePerHour)
            .GreaterThan(0).When(x => x.PricePerHour.HasValue)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.Type)
            .Must(t => t == "Standard" || t == "Vip")
            .When(x => x.Type != null)
            .WithMessage("Type must be Standard or Vip");
    }
}