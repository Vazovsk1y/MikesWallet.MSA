using MassTransit;
using MikesWallet.Contracts.Events;

namespace MikesWallet.EmailNotifications.Worker.Consumers;

public class DepositCompletedConsumer(ILogger<DepositCompletedConsumer> logger) : IConsumer<DepositCompletedEvent>
{
    public Task Consume(ConsumeContext<DepositCompletedEvent> context)
    {
        logger.LogInformation("Deposit completed {Amount} {AccountName}.", context.Message.Amount, context.Message.AccountName);
        return Task.CompletedTask;
    }
}