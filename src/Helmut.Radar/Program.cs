using Helmut.General;
using Helmut.General.Infrastructure;
using Helmut.Radar.Features.Corresponder;
using Helmut.Radar.Features.Corresponder.Endpoints;
using Helmut.Radar.Features.Corresponder.Queues;
using Helmut.Radar.Features.Database;
using Helmut.Radar.Features.Extensions;
using Helmut.Radar.Features.VesselGeneratorService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Serilog;
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

builder.Services.AddDbContext<RadarDbContext>(options => options
    .UseSqlServer(builder.Configuration["DbContext:ConnectionString"])
    .LogTo(Log.Logger.Information)
    .EnableSensitiveDataLogging());

builder.Services.AddSingleton<IVesselGeneratorService, VesselGeneratorService>();

builder.Services.AddSingleton<CorresponderService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<CorresponderService>());
builder.Services.AddHostedService<ApplicationLifetimeHostedService>();

builder.Services.AddScoped<ICorresponderEnqueueEndpoint, CorresponderEnqueueEndpoint>();
builder.Services.AddScoped<ICorresponderUpdateStateEndpoint, CorresponderUpdateStateEndpoint>();

builder.Services.AddSingleton<ICorresponderTaskQueue>(new CorresponderTaskQueue());
builder.Services.AddSingleton<ICorresponderStateTaskQueue>(new CorresponderStateTaskQueue(10, BoundedChannelFullMode.DropOldest));

var app = builder.Build();

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<RadarDbContext>();
await context.Database.MigrateAsync();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
