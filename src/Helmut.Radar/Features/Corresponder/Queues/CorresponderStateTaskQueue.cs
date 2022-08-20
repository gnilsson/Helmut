using Helmut.General;
using Helmut.Radar.Features.Corresponder.Models;
using System.Threading.Channels;

namespace Helmut.Radar.Features.Corresponder.Queues;

public class CorresponderStateTaskQueue : BackgroundTaskQueue<Func<CorresponderState, CorresponderState>>, ICorresponderStateTaskQueue
{
    public CorresponderStateTaskQueue(int capacity, BoundedChannelFullMode mode = BoundedChannelFullMode.Wait) : base(capacity, mode)
    {
    }
}
