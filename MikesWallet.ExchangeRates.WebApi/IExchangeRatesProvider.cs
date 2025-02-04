using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MikesWallet.Contracts.Cache;

namespace MikesWallet.ExchangeRates.WebApi;

public interface IExchangeRatesProvider
{
    Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(CancellationToken cancellationToken = default);
}

public record ExchangeRate(string FromCurrencyCode, string ToCurrencyCode, decimal Rate);

internal class RandomExchangeRatesProvider(IDistributedCache cache) : IExchangeRatesProvider
{
    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var currenciesStr = await cache.GetStringAsync("Currencies", cancellationToken);
        if (string.IsNullOrWhiteSpace(currenciesStr))
        {
            throw new ArgumentNullException(currenciesStr);
        }
        
        var currencies = JsonSerializer.Deserialize<List<CurrencyCacheModel>>(currenciesStr)!;
        
        var result = new List<ExchangeRate>();
        foreach (var t in currencies)
        {
            result.AddRange(
                from t1 in currencies 
                where t.AlphabeticCode != t1.AlphabeticCode 
                select new ExchangeRate(t.AlphabeticCode, t1.AlphabeticCode, (decimal)Random.Shared.NextDouble())
            );
        }
        
        return result;
    }
}