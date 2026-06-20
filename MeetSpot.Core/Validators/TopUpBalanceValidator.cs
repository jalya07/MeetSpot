using FluentValidation;
using MeetSpot.Core.DTOs.Balance;

namespace MeetSpot.Core.Validators;

public class TopUpBalanceValidator : AbstractValidator<TopUpBalanceDto>
{
    public TopUpBalanceValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Maximum top-up amount is 10000");

        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required")
            .Length(16).WithMessage("Card number must be 16 digits")
            .Matches(@"^\d+$").WithMessage("Card number must contain only digits");

        RuleFor(x => x.CardHolderName)
            .NotEmpty().WithMessage("Card holder name is required");

        RuleFor(x => x.ExpiryDate)
            .NotEmpty().WithMessage("Expiry date is required")
            .Matches(@"^(0[1-9]|1[0-2])\/\d{2}$").WithMessage("Expiry date must be in MM/YY format");

        RuleFor(x => x.Cvv)
            .NotEmpty().WithMessage("CVV is required")
            .Length(3, 4).WithMessage("CVV must be 3-4 digits")
            .Matches(@"^\d+$").WithMessage("CVV must contain only digits");
    }
}