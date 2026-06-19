using FluentValidation;
using MeetSpot.Core.DTOs.Booking;

namespace MeetSpot.Core.Validators;

public class UpdateBookingValidator : AbstractValidator<UpdateBookingDto>
{
    public UpdateBookingValidator()
    {
        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.StartTime.HasValue)
            .WithMessage("Start time must be in the future");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .When(x => x.EndTime.HasValue && x.StartTime.HasValue)
            .WithMessage("End time must be after start time");
    }
}