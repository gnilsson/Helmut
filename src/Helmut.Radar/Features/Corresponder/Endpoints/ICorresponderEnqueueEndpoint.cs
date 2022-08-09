using Helmut.Radar.Features.Corresponder.Models;

namespace Helmut.Radar.Features.Corresponder.Endpoints;

public interface ICorresponderEnqueueEndpoint
{
    Task ExecuteAsync(CorresponderEnqueueRequest request, CancellationToken cancellationToken);
}
