﻿// <auto-generated />
using System;
using Helmut.Radar.Features.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Helmut.Radar.Migrations
{
    [DbContext(typeof(RadarDbContext))]
    [Migration("20220820121204_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Helmut.Radar.Features.Database.CorresponderStateEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ExecutionCount")
                        .HasColumnType("int");

                    b.Property<int>("Mode")
                        .HasColumnType("int");

                    b.Property<int>("ProcessId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("States");
                });

            modelBuilder.Entity("Helmut.Radar.Features.Database.VesselEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CorresponderStateEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Group")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CorresponderStateEntityId");

                    b.ToTable("Vessels");
                });

            modelBuilder.Entity("Helmut.Radar.Features.Database.VesselEntity", b =>
                {
                    b.HasOne("Helmut.Radar.Features.Database.CorresponderStateEntity", null)
                        .WithMany("Vessels")
                        .HasForeignKey("CorresponderStateEntityId");
                });

            modelBuilder.Entity("Helmut.Radar.Features.Database.CorresponderStateEntity", b =>
                {
                    b.Navigation("Vessels");
                });
#pragma warning restore 612, 618
        }
    }
}
