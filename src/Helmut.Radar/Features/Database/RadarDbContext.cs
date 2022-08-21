using Boolkit;
using Helmut.Radar.Features.Corresponder.Enums;
using Helmut.Radar.Features.Database.Entities;
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

    class SequentialIdentifierValueGenerator : ValueGenerator<SequentialIdentifier2>
    {
        public override bool GeneratesTemporaryValues { get; }

        public override SequentialIdentifier2 Next(EntityEntry entry)
        {
            return SequentialIdentifier2.New();
        }
    }

    class SequentialIdentifierValueConverter : ValueConverter<SequentialIdentifier2, Guid>
    {
        public SequentialIdentifierValueConverter() : base(
            s => s,
            g => new SequentialIdentifier2(g))
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
        //var corresponderStateEntity = modelBuilder.Entity<CorresponderStateEntity>();
        //corresponderStateEntity
        //    .Property(p => p.Id)
        //    .HasConversion<SequentialIdentifierValueConverter>()
        //    .HasValueGenerator<SequentialIdentifierValueGenerator>();

        var corresponderStateEntity = modelBuilder.Entity<CorresponderStateEntity>();
        corresponderStateEntity
            .Property(p => p.Id)
            .HasConversion<NewIdValueConverter>()
            .HasValueGenerator<NewIdValueGenerator>();

        var vesselEntity = modelBuilder.Entity<VesselEntity>();
        vesselEntity
            .Property(p => p.Id)
            .HasConversion<IdentifierValueConverter>();

        //vesselEntity
        //    .Property(p => p.CorresponderStateEntityId)
        //    .HasConversion<SequentialIdentifierValueConverter>()
        //    .IsRequired();

        corresponderStateEntity
            .HasMany(b => b.Vessels)
            .WithOne()
            .IsRequired();
    }

    public DbSet<CorresponderStateEntity> States => Set<CorresponderStateEntity>();
    public DbSet<VesselEntity> Vessels => Set<VesselEntity>();
}
