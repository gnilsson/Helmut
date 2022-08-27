using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helmut.Radar.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessId = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    ExecutionCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vessels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    CorresponderStateEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vessels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vessels_States_CorresponderStateEntityId",
                        column: x => x.CorresponderStateEntityId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_CorresponderStateEntityId",
                table: "Vessels",
                column: "CorresponderStateEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vessels");

            migrationBuilder.DropTable(
                name: "States");
        }
    }
}
