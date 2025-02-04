namespace MikesWallet.Contracts.Events;

public class WithdrawalCompletedEvent : EventBase
{
    public required string AccountName { get; init; }
    
    public required string AccountCurrency { get; init; }
    
    public required decimal Amount { get; init; }
    
    public DateTimeOffset Date { get; init; }
}