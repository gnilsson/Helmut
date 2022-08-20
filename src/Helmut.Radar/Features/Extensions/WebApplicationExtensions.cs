using Helmut.Radar.Features.Corresponder.Endpoints;
using Helmut.Radar.Features.Corresponder.Models;
using Microsoft.AspNetCore.Mvc;

namespace Helmut.Radar.Features.Extensions;

internal static class WebApplicationExtensions
{
    internal static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/radar/enqueue", async (
            [FromBody] CorresponderEnqueueRequest request,
            [FromServices] ICorresponderEnqueueEndpoint endpoint,
            CancellationToken cancellationToken) =>
        {
            await endpoint.ExecuteAsync(request, cancellationToken);

            return Results.Accepted();
        });

        app.MapPost("/radar/state", async (
            [FromBody] CorresponderUpdateStateRequest request,
            [FromServices] ICorresponderUpdateStateEndpoint endpoint,
            CancellationToken cancellationToken) =>
        {
            await endpoint.ExecuteAsync(request, cancellationToken);

            return Results.Accepted();
        });
    }
}
