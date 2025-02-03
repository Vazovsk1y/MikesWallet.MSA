using FluentValidation;

namespace MikesWallet.Accounts.WebApi.Requests;

public record AccountTransferRequest(decimal Amount);

public class AccountTransferRequestValidator : AbstractValidator<AccountTransferRequest>
{
    public AccountTransferRequestValidator()
    {
        RuleFor(e => e.Amount)
            .InclusiveBetween(1, decimal.MaxValue);
    }
}