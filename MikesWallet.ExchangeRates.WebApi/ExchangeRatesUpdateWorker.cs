using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MikesWallet.Contracts.Cache;

namespace MikesWallet.ExchangeRates.WebApi;

public class ExchangeRatesUpdateWorker(
    IServiceScopeFactory scopeFactory,
    IDistributedCache cache) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(20);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        
        do
        {
            using var scope = scopeFactory.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<IExchangeRatesProvider>();
            
            var result = await provider.GetExchangeRatesAsync(stoppingToken);
            
            await cache.SetStringAsync("ExchangeRates", JsonSerializer.Serialize(result.Select(e => new ExchangeRateCacheModel
            {
                FromCurrencyCode = e.FromCurrencyCode,
                ToCurrencyCode = e.ToCurrencyCode,
                Rate = e.Rate,
            })), new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = Interval,
            }, stoppingToken);
            
        } while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
    }
}