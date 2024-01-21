using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerUsageCUI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThreePhaseMeterSet",
                columns: table => new
                {
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeviceName = table.Column<string>(type: "TEXT", nullable: false),
                    ActivePowerPhase1 = table.Column<float>(type: "REAL", nullable: false),
                    ActivePowerPhase2 = table.Column<float>(type: "REAL", nullable: false),
                    ActivePowerPhase3 = table.Column<float>(type: "REAL", nullable: false),
                    TotalActivePower = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreePhaseMeterSet", x => new { x.DateTime, x.DeviceName });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreePhaseMeterSet");
        }
    }
}
