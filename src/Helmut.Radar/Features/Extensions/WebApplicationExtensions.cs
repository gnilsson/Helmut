using Helmut.Radar.Features.Corresponder.Endpoints;
using Helmut.Radar.Features.Corresponder.Models;
using Helmut.Radar.Features.Database;
using Helmut.Radar.Features.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmut.Radar.Features.Extensions;

internal static class WebApplicationExtensions
{
    internal static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/radar/enqueue", PostRadarEnqueue);
        app.MapPost("/radar/states", PostRadarStates);
        app.MapGet("/radar/states", GetRadarStates);

        async Task<IResult> PostRadarEnqueue(
            [FromBody] CorresponderEnqueueRequest request,
            [FromServices] ICorresponderEnqueueEndpoint endpoint,
            CancellationToken cancellationToken)
        {
            await endpoint.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

            return Results.Accepted();
        };

        async Task<IResult> PostRadarStates(
            [FromBody] CorresponderUpdateStateRequest request,
            [FromServices] ICorresponderUpdateStateEndpoint endpoint,
            CancellationToken cancellationToken)
        {
            await endpoint.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

            return Results.Accepted();
        }

        IResult GetRadarStates([FromServices] RadarDbContext dbContext, CancellationToken cancellationToken)
        {
            return Results.Ok(YieldState());

            async IAsyncEnumerable<Response> YieldState()
            {
                await foreach (var state in dbContext.States
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Include(x => x.Vessels)
                    .AsAsyncEnumerable()
                    .ConfigureAwait(false))
                {
                    yield return new Response { Id = state.Id.ToString(), ProcessId = state.ProcessId, Mode = (int)state.Mode, };
                }
            }
        }
    }
}

public class Response
{
    public string Id { get; init; } = default!;
    public int ProcessId { get; init; }
    public int Mode { get; init; }
}