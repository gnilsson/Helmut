using Helmut.General.Models;
using Helmut.Radar.Features.Corresponder.Enums;
using System.Collections.Immutable;

namespace Helmut.Radar.Features.Corresponder.Models;

public record CorresponderState(int Id, CorresponderMode Mode, ImmutableArray<Vessel>? Vessels, int ExecutionCount);
