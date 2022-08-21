using Boolkit;
using Helmut.Radar.Features.Corresponder.Enums;
using MassTransit;

namespace Helmut.Radar.Features.Database.Entities;

public class CorresponderStateEntity
{
    public CorresponderStateEntity()
    {
        Vessels = new HashSet<VesselEntity>();
    }

    //public SequentialIdentifier Id { get; set; }
    public NewId Id { get; init; }
    public int ProcessId { get; init; }
    public CorresponderMode Mode { get; init; }
    public ICollection<VesselEntity> Vessels { get; init; }
    public int ExecutionCount { get; init; }
}
