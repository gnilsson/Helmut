using Azure.Messaging.ServiceBus;
using Boolkit;
using Helmut.General.Models;
using Helmut.Radar.Features.Corresponder.Enums;
using Helmut.Radar.Features.Corresponder.Models;
using Helmut.Radar.Features.Corresponder.Queues;
using Helmut.Radar.Features.Database;
using Helmut.Radar.Features.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Helmut.Radar.Features.Corresponder;

public sealed class CorresponderService : BackgroundService
{
    private readonly ILogger<CorresponderService> _logger;
    private readonly ServiceBusClient _client;
    private readonly IConfiguration _configuration;
    private readonly ICorresponderTaskQueue _taskQueue;
    private readonly ICorresponderStateTaskQueue _stateTaskQueue;
    private readonly IServiceScopeFactory _scopeFactory;

    public CorresponderService(
        ILogger<CorresponderService> logger,
        ServiceBusClient client,
        IConfiguration configuration,
        ICorresponderTaskQueue taskQueue,
        ICorresponderStateTaskQueue stateTaskQueue,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _client = client;
        _configuration = configuration;
        _taskQueue = taskQueue;
        _stateTaskQueue = stateTaskQueue;
        _scopeFactory = scopeFactory;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Corresponder Service is stopping.");

        await base.StopAsync(stoppingToken).ConfigureAwait(false);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var sender = _client.CreateSender(_configuration["AzureServiceBus:QueueName"]);

        var executionCount = 0;

        var state = new CorresponderState(0, CorresponderMode.Inactive, ImmutableArray<Vessel>.Empty, executionCount);

        _ = Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var correspond = await _taskQueue.DequeueTaskAsync(stoppingToken).ConfigureAwait(false);

                try
                {
                    await correspond(sender, state, stoppingToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred executing {Task}.", nameof(correspond));
                }

                Interlocked.Increment(ref executionCount);
            }
        }, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var update = await _stateTaskQueue.DequeueTaskAsync(stoppingToken).ConfigureAwait(false);

            var previousState = state;
            state = update(state);
            var previousCount = executionCount;
            executionCount = 0;

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();

                await using var dbContext = scope.ServiceProvider.GetRequiredService<RadarDbContext>();

                var vessels = await YieldVessel(previousState.Vessels, dbContext.Vessels, stoppingToken)
                    .ToArrayAsync(stoppingToken)
                    .ConfigureAwait(false);

                var stateEntity = new CorresponderStateEntity
                {
                    ProcessId = previousState.Id,
                    Mode = previousState.Mode,
                    Vessels = vessels,
                    ExecutionCount = previousCount,
                };

                await dbContext.States.AddAsync(stateEntity, stoppingToken).ConfigureAwait(false);

                await dbContext.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured updating database.");
            }
        }
    }

    private static async IAsyncEnumerable<VesselEntity> YieldVessel(
        ImmutableArray<Vessel>? stateVessels,
        DbSet<VesselEntity> dbSet,
        [EnumeratorCancellation] CancellationToken stoppingToken)
    {
        if (!(stateVessels is not null and { Length: > 0 } vessels)) yield break;

        var vesselIds = vessels
            .Select(x => x.Id)
            .ToArray();

        var existingVessels = await dbSet
            .AsNoTracking()
            .Where(x => vesselIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToArrayAsync(stoppingToken)
            .ConfigureAwait(false);

        foreach (var vessel in vessels)
        {
            if (existingVessels.Contains(vessel.Id)) continue;

            yield return new VesselEntity
            {
                Id = vessel.Id,
                Name = vessel.Affinity.Name,
                Group = vessel.Affinity.Group,
                Latitude = vessel.Coordinates.Latitude,
                Longitude = vessel.Coordinates.Longitude,
            };
        }
    }
}
