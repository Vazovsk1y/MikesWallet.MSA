using FluentValidation;

namespace MikesWallet.Accounts.WebApi.Requests;

public record AccountDepositRequest(decimal Amount);

public class AccountDepositRequestValidator : AbstractValidator<AccountDepositRequest>
{
    public AccountDepositRequestValidator()
    {
        RuleFor(e => e.Amount)
            .InclusiveBetween(1, decimal.MaxValue);
    }
}