using Azure.Messaging.ServiceBus;
using Helmut.General;
using Helmut.Operations.Features.MessageProcessor.Contracts;
using System.Threading.Channels;

namespace Helmut.Operations.Features.MessageProcessor;

public class MessageProcessorTaskQueue : BackgroundTaskQueue<Func<ServiceBusProcessor, Func<CancellationToken, Task>, CancellationToken, ValueTask>>, IMessageProcessorTaskQueue
{
    public MessageProcessorTaskQueue(int capacity, BoundedChannelFullMode mode = BoundedChannelFullMode.Wait) : base(capacity, mode)
    {
    }
}
