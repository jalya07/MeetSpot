using FluentValidation;
using MeetSpot.Core.DTOs.Blog;

namespace MeetSpot.Core.Validators;

public class CreateBlogPostValidator : AbstractValidator<CreateBlogPostDto>
{
    public CreateBlogPostValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MinimumLength(50).WithMessage("Content must be at least 50 characters");
    }
}