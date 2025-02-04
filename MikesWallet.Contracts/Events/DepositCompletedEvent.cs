namespace MikesWallet.Contracts.Events;

public class DepositCompletedEvent : EventBase
{
    public required string AccountName { get; init; }
    
    public required string AccountCurrency { get; init; }
    
    public required decimal Amount { get; init; }
    
    public required DateTimeOffset Date { get; init; }
}