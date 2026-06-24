using FluentValidation;
using MeetSpot.Core.DTOs.User;

namespace MeetSpot.Core.Validators;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .When(x => x.FirstName != null);

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .When(x => x.LastName != null);
    }
}