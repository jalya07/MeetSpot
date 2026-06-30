using FluentValidation;
using MeetSpot.Core.DTOs.Review;

namespace MeetSpot.Core.Validators;

public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Review text is required")
            .MinimumLength(20).WithMessage("Review must be at least 20 characters")
            .MaximumLength(500).WithMessage("Review must not exceed 500 characters");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
    }
}