using FluentValidation;

namespace MikesWallet.Accounts.WebApi.Requests;

public record AccountWithdrawRequest(decimal Amount);

public class AccountWithdrawRequestValidator : AbstractValidator<AccountWithdrawRequest>
{
    public AccountWithdrawRequestValidator()
    {
        RuleFor(e => e.Amount)
            .InclusiveBetween(1, decimal.MaxValue);
    }
}