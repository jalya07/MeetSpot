using FluentValidation;
using MeetSpot.Core.DTOs.Booking;

namespace MeetSpot.Core.Validators;

public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200);

        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("Room is required");

        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Start time must be in the future");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");
    }
}