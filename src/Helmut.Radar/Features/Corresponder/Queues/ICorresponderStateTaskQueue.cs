using Helmut.General;
using Helmut.Radar.Features.Corresponder.Models;

namespace Helmut.Radar.Features.Corresponder.Queues;

public interface ICorresponderStateTaskQueue : IBackgroundTaskQueue<Func<CorresponderState, CorresponderState>>
{ }
