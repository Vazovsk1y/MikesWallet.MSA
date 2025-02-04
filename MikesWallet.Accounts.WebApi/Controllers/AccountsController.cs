using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MikesWallet.Accounts.WebApi.DAL;
using MikesWallet.Accounts.WebApi.DAL.Models;
using MikesWallet.Accounts.WebApi.Infrastructure;
using MikesWallet.Accounts.WebApi.Requests;
using MikesWallet.Contracts.Cache;

namespace MikesWallet.Accounts.WebApi.Controllers;

[Authorize]
public class AccountsController(ApplicationDbContext dbContext, TimeProvider timeProvider) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        AccountCreateRequest request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var account = new Account()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Currency = request.Currency,
            CreationDateTime = timeProvider.GetUtcNow(),
            Balance = 0,
            UserId = GetRequiredCurrentUserId(),
        };
        
        dbContext.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Ok(account.Id);
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAccounts(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await dbContext
            .Accounts
            .OrderBy(e => e.CreationDateTime)
            .Where(a => a.UserId == GetRequiredCurrentUserId())
            .Select(e => new
            {
                e.Id,
                e.Name,
                Currency = e.Currency.ToString(),
                e.CreationDateTime,
                e.Balance,
            })
            .ToListAsync(cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("{accountId}/operations")]
    public async Task<IActionResult> GetAccountOperations(Guid accountId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await dbContext
            .Operations
            .OrderBy(e => e.CreationDateTime)
            .Include(e => ((Transfer)e).From)
            .Include(e => ((Transfer)e).To)
            .Where(e => ((Deposit)e).AccountId == accountId ||
                        ((Withdrawal)e).AccountId == accountId ||
                        (((Transfer)e).AccountFromId == accountId && !e.Income) || (((Transfer)e).AccountToId == accountId && e.Income))
            .Select(e => e.ToResponse())
            .ToListAsync(cancellationToken);
        
        return Ok(result);
    }


    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> DepositAccount(
        Guid accountId, 
        AccountDepositRequest request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var account = await dbContext.Accounts.FirstAsync(e => e.Id == accountId, cancellationToken);
            account.Balance += request.Amount;

            var operation = new Deposit
            {
                AccountId = account.Id,
                Id = Guid.NewGuid(),
                CreationDateTime = timeProvider.GetUtcNow(),
                Income = true,
                Amount = request.Amount,
                Commission = 0,
                Type = OperationType.Deposit,
            };
            
            dbContext.Deposits.Add(operation);
            
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync();
            
            // TODO: Push message to broker.
            
            return Ok(operation.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    [HttpPost("{accountId}/withdraw")]
    public async Task<IActionResult> WithdrawAccount(
        Guid accountId, 
        AccountWithdrawRequest request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var account = await dbContext.Accounts.FirstAsync(e => e.Id == accountId, cancellationToken);
            
            var currentUserId = GetRequiredCurrentUserId();
            if (account.UserId != currentUserId)
            {
                return Forbid();
            }
            
            account.Balance -= request.Amount;

            var operation = new Withdrawal
            {
                AccountId = account.Id,
                Id = Guid.NewGuid(),
                CreationDateTime = timeProvider.GetUtcNow(),
                Income = false,
                Amount = request.Amount,
                Commission = 0,
                Type = OperationType.Withdrawal,
            };
            
            dbContext.Withdrawals.Add(operation);
            
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync();
            
            // TODO: Push message to broker.
            
            return Ok(operation.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPost("{accountFromId}/transfer/{accountToId}")]
    public async Task<IActionResult> TransferTo(
        Guid accountFromId, 
        Guid accountToId, 
        AccountTransferRequest request,
        IDistributedCache cache,
        CancellationToken cancellationToken)
    {

        cancellationToken.ThrowIfCancellationRequested();

        if (accountFromId == accountToId)
        {
            return BadRequest();
        }
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            const decimal commissionRate = 0.05M;

            var fromAccount = await dbContext.Accounts.FirstAsync(e => e.Id == accountFromId, cancellationToken);
            if (fromAccount.UserId != GetRequiredCurrentUserId())
            {
                return Forbid();
            }
            
            var toAccount = await dbContext.Accounts.FirstAsync(e => e.Id == accountToId, cancellationToken);

            decimal exchangeRate = 1;
            if (fromAccount.Currency != toAccount.Currency)
            {
                var exchangeRatesStr = await cache.GetStringAsync("ExchangeRates", cancellationToken);
                if (string.IsNullOrWhiteSpace(exchangeRatesStr))
                {
                    return BadRequest("Обменные курсы не найдены в кеше.");
                }

                var rates = JsonSerializer.Deserialize<List<ExchangeRateCacheModel>>(exchangeRatesStr);
                var rate = rates!.FirstOrDefault(e => e.FromCurrencyCode == fromAccount.Currency.ToString() && 
                                                      e.ToCurrencyCode == toAccount.Currency.ToString());
                if (rate is null)
                {
                    return BadRequest("Обменный курс не найден.");
                }
                
                exchangeRate = rate.Rate;
            }
            
            var currentDateTime = timeProvider.GetUtcNow();
            
            var sub = new Transfer
            {
                AccountFromId = fromAccount.Id,
                AccountToId = toAccount.Id,
                ExchangeRate = exchangeRate,
                Id = Guid.NewGuid(),
                CreationDateTime = currentDateTime,
                Income = false,
                Amount = request.Amount + request.Amount * commissionRate,
                Commission = commissionRate,
                Type = OperationType.Transfer,
            };

            var add = new Transfer
            {
                AccountFromId = fromAccount.Id,
                AccountToId = toAccount.Id,
                ExchangeRate = exchangeRate,
                Id = Guid.NewGuid(),
                CreationDateTime = currentDateTime,
                Income = true,
                Amount = request.Amount * exchangeRate,
                Commission = decimal.Zero,
                Type = OperationType.Transfer,
            };
            
            fromAccount.Balance -= sub.Amount;
            toAccount.Balance += add.Amount;
            
            dbContext.Transfers.AddRange(sub, add);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync();
            
            // TODO: Push message to broker.
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        
        return Ok();
    }
}