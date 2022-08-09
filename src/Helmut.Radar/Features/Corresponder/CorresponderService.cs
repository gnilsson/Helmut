using Azure.Messaging.ServiceBus;
using Helmut.General.Models;
using Helmut.Radar.Features.Corresponder.Enums;
using Helmut.Radar.Features.Corresponder.Models;
using Helmut.Radar.Features.Corresponder.Queues;
using System.Collections.Immutable;

namespace Helmut.Radar.Features.Corresponder;

public sealed class CorresponderService : BackgroundService
{
    private readonly ILogger<CorresponderService> _logger;
    private readonly ServiceBusClient _client;
    private readonly IConfiguration _configuration;
    private readonly ICorresponderTaskQueue _taskQueue;
    private readonly ICorresponderStateTaskQueue _stateTaskQueue;
    private readonly Dictionary<int, CorresponderServiceState> _stateCollection = new();

    private int _executionCount = 0;

    public CorresponderService(
        ILogger<CorresponderService> logger,
        ServiceBusClient client,
        IConfiguration configuration,
        ICorresponderTaskQueue taskQueue,
        ICorresponderStateTaskQueue stateTaskQueue)
    {
        _logger = logger;
        _client = client;
        _configuration = configuration;
        _taskQueue = taskQueue;
        _stateTaskQueue = stateTaskQueue;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Corresponder Background Service is stopping.");

        await base.StopAsync(stoppingToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var sender = _client.CreateSender(_configuration["AzureServiceBus:QueueName"]);

        var state = new CorresponderServiceState(0, CorresponderMode.Inactive, ImmutableArray<Vessel>.Empty, 0);

        _ = Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await task(sender, state, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred executing {Task}.", nameof(task));
                }

                Interlocked.Increment(ref _executionCount);
            }
        }, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var update = await _stateTaskQueue.DequeueAsync(stoppingToken);

            _stateCollection.Add(state.Id, state with { ExecutionCount = _executionCount });

            _executionCount = 0;

            state = update(state);
        }
    }
}
