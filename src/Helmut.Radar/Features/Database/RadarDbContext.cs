using Helmut.Radar.Features.Corresponder.Enums;
using Microsoft.EntityFrameworkCore;

namespace Helmut.Radar.Features.Database;

public class RadarDbContext : DbContext
{
    public RadarDbContext(DbContextOptions<RadarDbContext> options) : base(options) { }

    public DbSet<CorresponderStateEntity> States => Set<CorresponderStateEntity>();
    public DbSet<VesselEntity> Vessels => Set<VesselEntity>();
}

public class CorresponderStateEntity
{
    public CorresponderStateEntity()
    {
        Vessels = new HashSet<VesselEntity>();
    }

    public Guid Id { get; init; }
    public int ProcessId { get; init; }
    public CorresponderMode Mode { get; init; }
    public ICollection<VesselEntity> Vessels { get; init; }
    public int ExecutionCount { get; init; }
}

public class VesselEntity
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Group { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}