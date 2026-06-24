using FluentValidation;
using MeetSpot.Core.DTOs.Auth;

namespace MeetSpot.Core.Validators;

public class VerifyTwoFactorValidator : AbstractValidator<VerifyTwoFactorDto>
{
    public VerifyTwoFactorValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required")
            .Length(6).WithMessage("Code must be 6 digits");
    }
}