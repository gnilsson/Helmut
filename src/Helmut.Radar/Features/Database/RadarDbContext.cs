using Helmut.Radar.Features.Database.Entities;
using Helmut.Radar.Features.IdConstructs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Helmut.Radar.Features.Database;

public class RadarDbContext : DbContext
{
    class NewIdValueGenerator : ValueGenerator<NewId>
    {
        public override bool GeneratesTemporaryValues { get; }

        public override NewId Next(EntityEntry entry)
        {
            return NewId.Next();
        }
    }

    class NewIdValueConverter : ValueConverter<NewId, Guid>
    {
        public NewIdValueConverter() : base(
            s => s.ToSequentialGuid(),
            g => g.ToNewIdFromSequential())
        { }
    }

    class IdentifierValueGenerator : ValueGenerator<Identifier>
    {
        public override bool GeneratesTemporaryValues { get; }

        public override Identifier Next(EntityEntry entry)
        {
            return Identifier.New();
        }
    }

    class IdentifierValueConverter : ValueConverter<Identifier, Guid>
    {
        public IdentifierValueConverter() : base(
            s => s,
            g => new Identifier(g))
        { }
    }

    public RadarDbContext(DbContextOptions<RadarDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var corresponderStateEntity = modelBuilder.Entity<CorresponderStateEntity>();
        corresponderStateEntity
            .Property(p => p.Id)
            .HasConversion<NewIdValueConverter>()
            .HasValueGenerator<NewIdValueGenerator>();

        var vesselEntity = modelBuilder.Entity<VesselEntity>();
        vesselEntity
            .Property(p => p.Id)
            .HasConversion<IdentifierValueConverter>();

        corresponderStateEntity
            .HasMany(b => b.Vessels)
            .WithOne()
            .IsRequired();
    }

    public DbSet<CorresponderStateEntity> States => Set<CorresponderStateEntity>();
    public DbSet<VesselEntity> Vessels => Set<VesselEntity>();
}
