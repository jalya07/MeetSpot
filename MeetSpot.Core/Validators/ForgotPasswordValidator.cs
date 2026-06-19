using FluentValidation;
using MeetSpot.Core.DTOs.Auth;

namespace MeetSpot.Core.Validators;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}