using Helmut.Radar.Features.Corresponder.Models;

namespace Helmut.Radar.Features.Corresponder.Endpoints;

public interface ICorresponderUpdateStateEndpoint
{
    Task ExecuteAsync(CorresponderServiceStateRequest request, CancellationToken cancellationToken);
}
