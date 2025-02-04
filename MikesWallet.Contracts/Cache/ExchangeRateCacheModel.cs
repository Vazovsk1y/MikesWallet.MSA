namespace MikesWallet.Contracts.Cache;

public class ExchangeRateCacheModel
{
    public required string FromCurrencyCode { get; init; }
    
    public required string ToCurrencyCode { get; init; }
    
    public required decimal Rate { get; init; }
}