using Helmut.Radar.Features.IdConstructs;

namespace Helmut.Radar.Features.Database.Entities;

public class VesselEntity
{
    public Identifier Id { get; init; }
    public string? Name { get; init; }
    public string? Group { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
