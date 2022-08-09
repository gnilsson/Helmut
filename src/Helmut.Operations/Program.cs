using Helmut.Operations.Features.MessageProcessor;
using Helmut.Operations.Features.MessageProcessor.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAzureClients(azcfBuilder =>
{
    azcfBuilder.AddServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus"));
});

builder.Services.AddSingleton<MessageProcessorService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<MessageProcessorService>());
builder.Services.AddScoped<IMessageProcessorOperatorEndpoint, MessageProcessorOperatorEndpoint>();
builder.Services.AddSingleton<IMessageProcessorTaskQueue, MessageProcessorTaskQueue>();

var app = builder.Build();

app.MapPost("/operations/processor", async (
    [FromBody] MessageProcessorOperationRequest request,
    [FromServices] IMessageProcessorOperatorEndpoint endpoint,
    CancellationToken cancellationToken) =>
{
    await endpoint.ExecuteAsync(request, cancellationToken);

    return Results.Ok();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
