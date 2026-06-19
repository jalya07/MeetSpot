using FluentValidation;
using MeetSpot.Core.DTOs.Auth;

namespace MeetSpot.Core.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase letter")
            .Matches(@"\d").WithMessage("Password must contain a number")
            .Matches(@"[@$!%*?&]").WithMessage("Password must contain a special character");

        RuleFor(x => x.UserType)
            .NotEmpty()
            .Must(t => t == "Regular" || t == "Corporate" || t == "Premium")
            .WithMessage("UserType must be Regular, Corporate or Premium");
    }
}