using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class initoracle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vesselinfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Sid = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Course = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MagneticVariation = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    TotalDistanceGrd = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    ResetDistanceGrd = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    TotalDistanceWat = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    ResetDistanceWat = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    WindDirection = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    WindSpeed = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    WaveHeight = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    WaveDirection = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Temperature = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Pressure = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Weather = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    Visibility = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    WaterSpeed = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    GroundSpeed = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BowDraft = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    AsternDraft = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    PortDraft = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    StarBoardDraft = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Trim = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Heel = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Draft = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Depth = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DepthOffset = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MESFOC = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEHFOConsumption = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEMDOConsumption = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGSFOC = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGHFOConsumption = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGMDOConsumption = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRSFOC = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRHFOConsumption = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRMDOConsumption = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    slip = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEPower = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Torque = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MERpm = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Thrust = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    createtime = table.Column<DateTime>(name: "create_time", type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updatetime = table.Column<DateTime>(name: "update_time", type: "TIMESTAMP(7)", nullable: true),
                    deletetime = table.Column<DateTime>(name: "delete_time", type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vesselinfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vesselinfo");
        }
    }
}