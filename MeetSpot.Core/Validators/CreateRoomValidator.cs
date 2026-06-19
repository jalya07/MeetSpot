using FluentValidation;
using MeetSpot.Core.DTOs.Room;

namespace MeetSpot.Core.Validators;

public class CreateRoomValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Room name is required")
            .MaximumLength(100);

        RuleFor(x => x.Floor)
            .GreaterThan(0).WithMessage("Floor must be greater than 0");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0");

        RuleFor(x => x.PricePerHour)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => t == "Standard" || t == "Vip")
            .WithMessage("Type must be Standard or Vip");
    }
}