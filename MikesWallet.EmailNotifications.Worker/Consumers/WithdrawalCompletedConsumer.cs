using MassTransit;
using MikesWallet.Contracts.Events;

namespace MikesWallet.EmailNotifications.Worker.Consumers;

public class WithdrawalCompletedConsumer(ILogger<WithdrawalCompletedConsumer> logger) : IConsumer<WithdrawalCompletedEvent>
{
    public Task Consume(ConsumeContext<WithdrawalCompletedEvent> context)
    {
        logger.LogInformation("Withdrawal completed {Amount} {AccountName}.", context.Message.Amount, context.Message.AccountName);
        return Task.CompletedTask;
    }
}