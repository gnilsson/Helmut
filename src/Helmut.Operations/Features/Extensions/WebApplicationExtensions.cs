using Helmut.Operations.Features.Database;
using Helmut.Operations.Features.MessageProcessor.Contracts;
using Helmut.Operations.Features.MessageProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading;

namespace Helmut.Operations.Features.Extensions;

internal static class WebApplicationExtensions
{
    internal static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/operations/ping", GetOperationsPing);
        app.MapPost("/operations/processor", PostOperationsProcessor);

        static async Task<IResult> GetOperationsPing([FromServices] IPinger ctx)
        {
            await ctx.PingAsync();

            return Results.Ok();
        }

        static async Task<IResult> PostOperationsProcessor(
            [FromBody] MessageProcessorOperationRequest request,
            [FromServices] IMessageProcessorOperatorEndpoint endpoint,
            HttpResponse response,
            CancellationToken cancellationToken)
        {
            await endpoint.ExecuteAsync(request, cancellationToken);

            return Results.Ok(JsonSerializer.Serialize(request));
        }
    }
}
