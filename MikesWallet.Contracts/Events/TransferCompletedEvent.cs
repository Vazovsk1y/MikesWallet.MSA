namespace MikesWallet.Contracts.Events;

public class TransferCompletedEvent : EventBase
{
    public required string AccountFromName { get; init; }
    
    public required string AccountFromCurrency { get; init; }
    
    public required string AccountToName { get; init; }
    
    public required string AccountToCurrency { get; init; }
    
    public required bool IsIncome { get; init; }
    
    public required decimal Amount { get; init; }
    
    public required DateTimeOffset Date { get; init; }
}