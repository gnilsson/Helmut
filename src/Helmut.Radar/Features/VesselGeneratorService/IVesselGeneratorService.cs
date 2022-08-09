using Helmut.General.Models;

namespace Helmut.Radar.Features.VesselGeneratorService;

public interface IVesselGeneratorService
{
    IEnumerable<Vessel>? GenerateFreshVessels(int count);
}
