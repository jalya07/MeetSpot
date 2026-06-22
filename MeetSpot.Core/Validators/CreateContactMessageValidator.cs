using FluentValidation;
using MeetSpot.Core.DTOs.Contact;

namespace MeetSpot.Core.Validators;

public class CreateContactMessageValidator : AbstractValidator<CreateContactMessageDto>
{
    public CreateContactMessageValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required")
            .MinimumLength(10).WithMessage("Message must be at least 10 characters");
    }
}