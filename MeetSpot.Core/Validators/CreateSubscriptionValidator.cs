using FluentValidation;
using MeetSpot.Core.DTOs.Subscription;

namespace MeetSpot.Core.Validators;

public class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionDto>
{
    public CreateSubscriptionValidator()
    {
        RuleFor(x => x.Plan)
            .NotEmpty()
            .Must(p => p == "Monthly" || p == "Annual")
            .WithMessage("Plan must be Monthly or Annual");
    }
}