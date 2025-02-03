using System.Text.Json.Serialization;
using FluentValidation;
using MikesWallet.Accounts.WebApi.DAL.Models;

namespace MikesWallet.Accounts.WebApi.Requests;

public record AccountCreateRequest(
    string Name, 
    [property: JsonConverter(typeof(JsonStringEnumConverter))] 
    Currency Currency);

public class AccountCreateRequestValidator : AbstractValidator<AccountCreateRequest>
{
    public AccountCreateRequestValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(70);
    }
}