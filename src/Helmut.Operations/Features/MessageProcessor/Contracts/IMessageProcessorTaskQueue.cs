using Azure.Messaging.ServiceBus;
using Helmut.General;

namespace Helmut.Operations.Features.MessageProcessor.Contracts;

public interface IMessageProcessorTaskQueue : IBackgroundTaskQueue<Func<ServiceBusProcessor, Func<CancellationToken, Task>, CancellationToken, ValueTask>>
{ }
