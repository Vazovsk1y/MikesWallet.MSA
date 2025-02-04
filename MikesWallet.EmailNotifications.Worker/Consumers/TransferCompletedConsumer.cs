using MassTransit;
using MikesWallet.Contracts.Events;

namespace MikesWallet.EmailNotifications.Worker.Consumers;

public class TransferCompletedConsumer(ILogger<TransferCompletedConsumer> logger) : IConsumer<TransferCompletedEvent>
{
    public Task Consume(ConsumeContext<TransferCompletedEvent> context)
    {
        logger.LogInformation("Transfer completed {Amount} {AccountFrom}.", context.Message.Amount,
            context.Message.AccountFromName);
        return Task.CompletedTask;
    }
}