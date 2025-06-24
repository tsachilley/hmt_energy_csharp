using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class addenergies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "energy_battery",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SOC = table.Column<decimal>(type: "DECIMAL(8,4)", precision: 8, scale: 4, nullable: true),
                    SOH = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: true),
                    MaxTEMP = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    MaxTEMPBox = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MaxTEMPNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MinTEMP = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    MinTEMPBox = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MinTEMPNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MaxVoltage = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    MaxVoltageBox = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MaxVoltageNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MinVoltage = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    MinVoltageBox = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MinVoltageNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_battery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "energy_flowmeter",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ConsAct = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    ConsAcc = table.Column<decimal>(type: "DECIMAL(14,4)", precision: 14, scale: 4, nullable: true),
                    Temperature = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    Density = table.Column<decimal>(type: "DECIMAL(9,4)", precision: 9, scale: 4, nullable: true),
                    DeviceType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FuelType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_flowmeter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "energy_generator",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IsRuning = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    RPM = table.Column<decimal>(type: "DECIMAL(6,0)", precision: 6, scale: 0, nullable: true),
                    StartPressure = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    ControlPressure = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    ScavengingPressure = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    LubePressure = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    LubeTEMP = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    FuelPressure = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    FuelTEMP = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    FreshWaterPressure = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    FreshWaterTEMPIn = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    FreshWaterTEMPOut = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CoolingWaterPressure = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    CoolingWaterTEMPIn = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CoolingWaterTEMPOut = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CylinderTEMP1 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CylinderTEMP2 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CylinderTEMP3 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CylinderTEMP4 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CylinderTEMP5 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CylinderTEMP6 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    SuperchargerTEMPIn = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    SuperchargerTEMPOut = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    ScavengingTEMP = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    BearingTEMP = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    BearingTEMPFront = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    BearingTEMPBack = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    Power = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    WindingTEMPL1 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    WindingTEMPL2 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    WindingTEMPL3 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    VoltageL1L2 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    VoltageL2L3 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    VoltageL1L3 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    FrequencyL1 = table.Column<decimal>(type: "DECIMAL(8,0)", precision: 8, scale: 0, nullable: true),
                    FrequencyL2 = table.Column<decimal>(type: "DECIMAL(8,0)", precision: 8, scale: 0, nullable: true),
                    FrequencyL3 = table.Column<decimal>(type: "DECIMAL(8,0)", precision: 8, scale: 0, nullable: true),
                    CurrentL1 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CurrentL2 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    CurrentL3 = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    ReactivePower = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    PowerFactor = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    LoadRatio = table.Column<decimal>(type: "DECIMAL(7,4)", precision: 7, scale: 4, nullable: true),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_generator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "energy_liquidlevel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Level = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: true),
                    Temperature = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_liquidlevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "energy_shaft",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Power = table.Column<decimal>(type: "DECIMAL(7,2)", precision: 7, scale: 2, nullable: true),
                    RPM = table.Column<decimal>(type: "DECIMAL(7,2)", precision: 7, scale: 2, nullable: true),
                    Torque = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    Thrust = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_shaft", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "energy_sternsealing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FrontTEMP = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    BackTEMP = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    BackLeftTEMP = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    BackRightTEMP = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_sternsealing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "energy_supplyunit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IsRuning = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Temperature = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    Pressure = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_supplyunit", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "energy_battery");

            migrationBuilder.DropTable(
                name: "energy_flowmeter");

            migrationBuilder.DropTable(
                name: "energy_generator");

            migrationBuilder.DropTable(
                name: "energy_liquidlevel");

            migrationBuilder.DropTable(
                name: "energy_shaft");

            migrationBuilder.DropTable(
                name: "energy_sternsealing");

            migrationBuilder.DropTable(
                name: "energy_supplyunit");
        }
    }
}
