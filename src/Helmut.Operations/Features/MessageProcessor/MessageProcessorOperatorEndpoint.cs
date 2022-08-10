using Azure.Messaging.ServiceBus;
using Helmut.Operations.Features.MessageProcessor.Contracts;

namespace Helmut.Operations.Features.MessageProcessor;

public class MessageProcessorOperatorEndpoint : IMessageProcessorOperatorEndpoint
{
    private readonly IMessageProcessorTaskQueue _taskQueue;
    private readonly ILogger<MessageProcessorOperatorEndpoint> _logger;

    public MessageProcessorOperatorEndpoint(IMessageProcessorTaskQueue taskQueue, ILogger<MessageProcessorOperatorEndpoint> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }

    public async ValueTask ExecuteAsync(MessageProcessorOperationRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing processor operation type {Type}", request.Type);

        var operationTask = request.Type switch
        {
            OperationType.Start => _taskQueue.QueueTaskAsync(StartAsync),
            OperationType.StopProcessor => _taskQueue.QueueTaskAsync(StopProcessorAsync),
            OperationType.StopService => _taskQueue.QueueTaskAsync(StopServiceAsync),
            _ => ValueTask.CompletedTask
        };

        if (operationTask != ValueTask.CompletedTask)
        {
            await operationTask;
        }
    }

    private async ValueTask StartAsync(ServiceBusProcessor processor, Func<CancellationToken, Task> _, CancellationToken cancellationToken)
    {
        if (processor.IsProcessing) return;

        try
        {
            await processor.StartProcessingAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to boot message processing unit. ");
            return;
        }

        _logger.LogInformation("Booting message processing unit.");
    }

    private async ValueTask StopProcessorAsync(ServiceBusProcessor processor, Func<CancellationToken, Task> _, CancellationToken cancellationToken)
    {
        if (processor.IsProcessing is false) return;

        try
        {
            await processor.StopProcessingAsync(cancellationToken);
        }
        catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
        {
        }

        _logger.LogInformation("Aborting processing unit.");
    }

    private async ValueTask StopServiceAsync(ServiceBusProcessor processor, Func<CancellationToken, Task> stopServiceAsync, CancellationToken cancellationToken)
    {
        await StopProcessorAsync(processor, null!, cancellationToken);

        await stopServiceAsync(cancellationToken);
    }
}
