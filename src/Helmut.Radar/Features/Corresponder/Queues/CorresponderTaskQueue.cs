using Azure.Messaging.ServiceBus;
using Helmut.General;
using Helmut.Radar.Features.Corresponder.Models;
using System.Threading.Channels;

namespace Helmut.Radar.Features.Corresponder.Queues;

public class CorresponderTaskQueue : BackgroundTaskQueue<Func<ServiceBusSender, CorresponderServiceState, CancellationToken, ValueTask>>, ICorresponderTaskQueue
{
    public CorresponderTaskQueue(int capacity = 100, BoundedChannelFullMode mode = BoundedChannelFullMode.Wait) : base(capacity, mode)
    {
    }
}
