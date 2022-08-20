using Helmut.Radar.Features.Corresponder.Enums;

namespace Helmut.Radar.Features.Corresponder.Models;

public record CorresponderUpdateStateRequest(CorresponderMode Mode, int VesselCount);
