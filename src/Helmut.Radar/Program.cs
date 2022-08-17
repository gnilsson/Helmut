using Helmut.General;
using Helmut.General.Infrastructure;
using Helmut.Radar.Features.Corresponder;
using Helmut.Radar.Features.Corresponder.Endpoints;
using Helmut.Radar.Features.Corresponder.Models;
using Helmut.Radar.Features.Corresponder.Queues;
using Helmut.Radar.Features.VesselGeneratorService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseUrls(builder.Configuration["Docker:Url"]);
}

builder.LogWithSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAzureClients(azcfBuilder =>
{
    azcfBuilder.AddServiceBusClient(builder.Configuration["AzureServiceBus:ConnectionString"]);
});

builder.Services.AddSingleton<IVesselGeneratorService, VesselGeneratorService>();

builder.Services.AddSingleton<CorresponderService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<CorresponderService>());
builder.Services.AddHostedService<ApplicationLifetimeHostedService>();

builder.Services.AddScoped<ICorresponderEnqueueEndpoint, CorresponderEnqueueEndpoint>();
builder.Services.AddScoped<ICorresponderUpdateStateEndpoint, CorresponderUpdateStateEndpoint>();

builder.Services.AddSingleton<ICorresponderTaskQueue>(new CorresponderTaskQueue());
builder.Services.AddSingleton<ICorresponderStateTaskQueue>(new CorresponderStateTaskQueue(10, BoundedChannelFullMode.DropOldest));

var app = builder.Build();

app.MapPost("/radar/enqueue", async (
    [FromBody] CorresponderEnqueueRequest request,
    [FromServices] ICorresponderEnqueueEndpoint endpoint,
    CancellationToken cancellationToken) =>
{
    await endpoint.ExecuteAsync(request, cancellationToken);

    return Results.Accepted();
});

app.MapPost("/radar/state", async (
    [FromBody] CorresponderServiceStateRequest request,
    [FromServices] ICorresponderUpdateStateEndpoint endpoint,
    CancellationToken cancellationToken) =>
{
    await endpoint.ExecuteAsync(request, cancellationToken);

    return Results.Accepted();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
