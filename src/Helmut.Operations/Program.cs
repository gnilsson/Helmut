using Helmut.General;
using Helmut.General.Infrastructure;
using Helmut.Operations.Features.LocationTranscoder;
using Helmut.Operations.Features.MessageProcessor;
using Helmut.Operations.Features.MessageProcessor.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using System.Text.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Helmut.Operations.Features.Database;

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

builder.Services.AddSingleton<MessageProcessorService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<MessageProcessorService>());
builder.Services.AddHostedService<ApplicationLifetimeHostedService>();

builder.Services.AddScoped<IMessageProcessorOperatorEndpoint, MessageProcessorOperatorEndpoint>();
builder.Services.AddSingleton<IMessageProcessorTaskQueue>(new MessageProcessorTaskQueue());
builder.Services.AddSingleton<ILocationTranscoderService, LocationTranscoderService>();


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { "localhost:6379" },
}));
builder.Services.AddSingleton<GraphContext>();

var app = builder.Build();

app.MapGet("/operations/ping", async (
    [FromServices] GraphContext ctx) =>
{
    await ctx.PingAsync();

    return Results.Ok();
});

app.MapPost("/operations/processor", async (
    [FromBody] MessageProcessorOperationRequest request,
    [FromServices] IMessageProcessorOperatorEndpoint endpoint,
    HttpResponse response,
    CancellationToken cancellationToken) =>
{
    await endpoint.ExecuteAsync(request, cancellationToken);

    return Results.Ok(JsonSerializer.Serialize(request));
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();