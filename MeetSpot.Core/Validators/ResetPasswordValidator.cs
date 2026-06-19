using FluentValidation;
using MeetSpot.Core.DTOs.Auth;

namespace MeetSpot.Core.Validators;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase letter")
            .Matches(@"\d").WithMessage("Password must contain a number")
            .Matches(@"[@$!%*?&]").WithMessage("Password must contain a special character");
    }
}