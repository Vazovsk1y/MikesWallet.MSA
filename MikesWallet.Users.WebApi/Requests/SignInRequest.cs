using FluentValidation;

namespace MikesWallet.Users.WebApi.Requests;

public record SignInRequest(string Email, string Password);

internal class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(e => e.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(e => e.Password)
            .NotEmpty();
    }
}
