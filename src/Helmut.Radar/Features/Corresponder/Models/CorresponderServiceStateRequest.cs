using Helmut.Radar.Features.Corresponder.Enums;

namespace Helmut.Radar.Features.Corresponder.Models;

public record CorresponderServiceStateRequest(CorresponderMode Mode, int VesselCount);
