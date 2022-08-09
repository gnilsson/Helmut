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
        var operationTask = request.Type switch
        {
            OperationType.Start => _taskQueue.QueueBackgroundWorkItemAsync(StartAsync),
            OperationType.StopProcessor => _taskQueue.QueueBackgroundWorkItemAsync(StopProcessorAsync),
            OperationType.StopService => _taskQueue.QueueBackgroundWorkItemAsync(StopServiceAsync),
            _ => ValueTask.CompletedTask
        };

        if (operationTask != ValueTask.CompletedTask)
        {
            await operationTask;
        }
    }

    private async ValueTask StartAsync(ServiceBusProcessor processor, Func<CancellationToken, Task> _, CancellationToken cancellationToken)
    {
        if (processor.IsProcessing is false)
        {
            await processor.StartProcessingAsync(cancellationToken);

            _logger.LogInformation("Started message processing boot sequence.");
        }
    }

    private async ValueTask StopProcessorAsync(ServiceBusProcessor processor, Func<CancellationToken, Task> _, CancellationToken cancellationToken)
    {
        if (processor.IsProcessing)
        {
            await processor.StopProcessingAsync(cancellationToken);

            _logger.LogInformation("Aborting processing unit.");
        }
    }

    private async ValueTask StopServiceAsync(ServiceBusProcessor processor, Func<CancellationToken, Task> stopServiceAsync, CancellationToken cancellationToken)
    {
        await StopProcessorAsync(processor, null!, cancellationToken);

        await stopServiceAsync(cancellationToken);
    }
}
