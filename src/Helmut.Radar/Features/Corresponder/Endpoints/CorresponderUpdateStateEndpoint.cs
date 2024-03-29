﻿using Helmut.General.Models;
using Helmut.Radar.Features.Corresponder.Models;
using Helmut.Radar.Features.Corresponder.Queues;
using Helmut.Radar.Features.VesselGeneratorService;
using System.Collections.Immutable;
using System.Data;

namespace Helmut.Radar.Features.Corresponder.Endpoints;

public sealed class CorresponderUpdateStateEndpoint : ICorresponderUpdateStateEndpoint
{
    private readonly ICorresponderStateTaskQueue _taskQueue;
    private readonly ILogger<CorresponderUpdateStateEndpoint> _logger;
    private readonly IVesselGeneratorService _vesselGenerator;

    private CorresponderUpdateStateRequest _stateRequest = null!;

    public CorresponderUpdateStateEndpoint(ICorresponderStateTaskQueue taskQueue, ILogger<CorresponderUpdateStateEndpoint> logger, IVesselGeneratorService vesselGenerator)
    {
        _taskQueue = taskQueue;
        _logger = logger;
        _vesselGenerator = vesselGenerator;
    }

    public async Task ExecuteAsync(CorresponderUpdateStateRequest request, CancellationToken cancellationToken)
    {
        _stateRequest = request;

        await _taskQueue.QueueTaskAsync(UpdateState);

        _logger.LogInformation("Enqueued state with mode {Mode}", request.Mode);
    }

    private CorresponderState UpdateState(CorresponderState currentState)
    {
        return UpdateState(currentState, _stateRequest, _vesselGenerator);
    }

    private static CorresponderState UpdateState(CorresponderState currentState, CorresponderUpdateStateRequest request, IVesselGeneratorService vesselGenerator)
    {
        var freshVessels = vesselGenerator.GenerateFreshVessels(request.VesselCount)?.ToImmutableArray();

        var allVessels = currentState.Vessels is null or { Length: 0 }
            ? freshVessels
            : AccumulateVessels(freshVessels, currentState.Vessels.Value);

        return new CorresponderState(currentState.Id + 1, request.Mode, allVessels, 0);
    }

    private static ImmutableArray<Vessel> AccumulateVessels(ImmutableArray<Vessel>? freshVessels, ImmutableArray<Vessel> vessels)
    {
        if (freshVessels is null)
        {
            if (vessels.Length == 0) return ImmutableArray<Vessel>.Empty;

            return vessels;
        }

        var builder = ImmutableArray.CreateBuilder<Vessel>();

        builder.AddRange(vessels);

        var affinities = vessels.Select(x => x.Affinity).ToArray();

        builder.AddRange(freshVessels.Value.Where(x => affinities.Contains(x.Affinity) is false));

        return builder.ToImmutable();
    }
}
