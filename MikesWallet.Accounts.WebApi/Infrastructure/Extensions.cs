using MikesWallet.Accounts.WebApi.DAL.Models;

namespace MikesWallet.Accounts.WebApi.Infrastructure;

public static class Extensions
{
    public static object ToResponse(this Operation operation)
    {
        return operation switch
        {
            Deposit deposit => new
            {
                deposit.Id,
                deposit.Amount,
                deposit.CreationDateTime,
                deposit.Commission,
                deposit.Income,
                Type = deposit.Type.ToString(),
            },
            Withdrawal withdrawal => new
            {
                withdrawal.Id,
                withdrawal.Amount,
                withdrawal.CreationDateTime,
                withdrawal.Commission,
                withdrawal.Income,
                Type = withdrawal.Type.ToString(),
            },
            Transfer transfer => new
            {
                transfer.Id,
                transfer.Amount,
                transfer.CreationDateTime,
                transfer.Commission,
                transfer.Income,
                Type = transfer.Type.ToString(),
                transfer.ExchangeRate,
                From = new { transfer.From.Id, transfer.From.Name, Currency = transfer.From.Currency.ToString(), },
                To = new { transfer.To.Id, transfer.To.Name, Currency = transfer.To.Currency.ToString(), },
            },
            _ => throw new KeyNotFoundException()
        };
    }
}