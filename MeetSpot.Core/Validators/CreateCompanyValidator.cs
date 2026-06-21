using FluentValidation;
using MeetSpot.Core.DTOs.Company;

namespace MeetSpot.Core.Validators;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyDto>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(200);

        RuleFor(x => x.Domain)
            .NotEmpty().WithMessage("Domain is required")
            .Matches(@"^[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            .WithMessage("Invalid domain format (e.g. company.com)");

        RuleFor(x => x.ContractStartDate)
            .NotEmpty().WithMessage("Contract start date is required");
    }
}