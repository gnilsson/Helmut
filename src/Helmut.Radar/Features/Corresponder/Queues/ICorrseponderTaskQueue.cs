using Azure.Messaging.ServiceBus;
using Helmut.General;
using Helmut.Radar.Features.Corresponder.Models;

namespace Helmut.Radar.Features.Corresponder.Queues;

public interface ICorresponderTaskQueue : IBackgroundTaskQueue<Func<ServiceBusSender, CorresponderState, CancellationToken, ValueTask>>
{ }
