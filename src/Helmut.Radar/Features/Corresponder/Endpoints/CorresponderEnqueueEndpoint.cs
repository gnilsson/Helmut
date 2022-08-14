using Azure.Messaging.ServiceBus;
using Helmut.General.Models;
using Helmut.Radar.Features.Corresponder.Enums;
using Helmut.Radar.Features.Corresponder.Models;
using Helmut.Radar.Features.Corresponder.Queues;
using System.Collections.Immutable;
using System.Text.Json;

namespace Helmut.Radar.Features.Corresponder.Endpoints;

public sealed class CorresponderEnqueueEndpoint : ICorresponderEnqueueEndpoint
{
    private readonly ICorresponderTaskQueue _taskQueue;
    private readonly ILogger<CorresponderEnqueueEndpoint> _logger;

    public CorresponderEnqueueEndpoint(ICorresponderTaskQueue taskQueue, ILogger<CorresponderEnqueueEndpoint> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }

    public async Task ExecuteAsync(CorresponderEnqueueRequest request, CancellationToken cancellationToken)
    {
        for (int i = 0; i < request.Ammount; i++)
        {
            await _taskQueue.QueueTaskAsync(CreateMessagesAsync);

            _logger.LogInformation("Enqueued work item {First} of {Ammount}", i + 1, request.Ammount);
        }
    }

    private async ValueTask CreateMessagesAsync(ServiceBusSender sender, CorresponderServiceState state, CancellationToken cancellationToken)
    {
        if (state.Mode is CorresponderMode.Inactive || state.Vessels is null or { Length: 0 })
        {
            _logger.LogInformation("Corresponder status: Inactive.");
            return;
        }

        var vessels = YieldRandomVessel(state.Vessels.Value).ToArray();

        if (vessels.Length <= 0)
        {
            _logger.LogInformation("Radar is blind.");
            return;
        }

        using var messageBatch = await sender.CreateMessageBatchAsync(cancellationToken);

        foreach (var vessel in vessels)
        {
            var message = JsonSerializer.SerializeToUtf8Bytes(vessel);

            messageBatch.TryAddMessage(new ServiceBusMessage(message));

            _logger.LogInformation("Detected vessel!\nName: {Name}\nGroup: {Group}\nLatitude: {Latitude}\nLongitude: {Longitude}, delivering message.",
                vessel.Affinity.Name,
                vessel.Affinity.Group,
                vessel.Coordinates.Latitude,
                vessel.Coordinates.Longitude);
        }

        await sender.SendMessagesAsync(messageBatch, cancellationToken);
        return;
    }

    private static IEnumerable<Vessel> YieldRandomVessel(ImmutableArray<Vessel> vessels)
    {
        var random = new Random();

        foreach (var vessel in vessels)
        {
            var number = random.Next(1, 11);

            if (number == 10) yield return vessel;
        }
    }
}
