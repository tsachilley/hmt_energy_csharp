using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class AddEIER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SeaTemperature",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Energy",
                table: "energy_shaft",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Revolutions",
                table: "energy_shaft",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "engineroom_compositeboiler",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    BLRBurnerRunning = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRHFOService = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRDGOService = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRFOP1On = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRFOP2On = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRFOTempLow = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRFOPressHigh = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRFOTempHigh = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRDGOTempHigh = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRHFOTempHigh = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRGE1EXTempHigh = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    BLRGE2EXTempHigh = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_compositeboiler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_compressedairsupply",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MEStartPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEControlPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    ExhaustValuePress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_compressedairsupply", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_coolingfreshwater",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    LTCFWPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    CCLTCFWOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    LTCFW1Press = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    LTCFW2Press = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    LTCFW3Press = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    MEJWCOutPress = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_coolingfreshwater", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_coolingseawater",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CSWOutPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    CSWOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_coolingseawater", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_coolingwater",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MEJacketInPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEPressDrop = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEOutPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEJacketPressDrop = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEInTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEJacketCyl1OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEJacketCyl2OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEJacketCyl3OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEJacketCyl4OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEJacketCyl5OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEJacketCyl6OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECCCyl1OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECCCyl2OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECCCyl3OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECCCyl4OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECCCyl5OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECCCyl6OutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEACInPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEACInTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEACOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_coolingwater", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_cylinderluboil",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MEInTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_cylinderluboil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_exhaustgas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    METCInTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECyl1AfterTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECyl2AfterTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECyl3AfterTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECyl4AfterTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECyl5AfterTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECyl6AfterTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    METCOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEReceiverPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    METurbBackPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEACInTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEACOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_exhaustgas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_fo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MEInPressure = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEInTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEHPOPLeakage = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_fo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_fosupplyunit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    HFOService = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGOService = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_fosupplyunit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_luboil",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    METCInPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    METBSTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEMBTBInPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEPistonCOInPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEInTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECYL1PistonCOOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECYL2PistonCOOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECYL3PistonCOOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECYL4PistonCOOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECYL5PistonCOOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECYL6PistonCOOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECYL1PistonCOOutNoFlow = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    MECYL2PistonCOOutNoFlow = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    MECYL3PistonCOOutNoFlow = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    MECYL4PistonCOOutNoFlow = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    MECYL5PistonCOOutNoFlow = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    MECYL6PistonCOOutNoFlow = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    METCOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEWaterHigh = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_luboil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_luboilpurifying",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MEFilterPressHigh = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_luboilpurifying", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_maingeneratorset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DGCFWInPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGStartAirPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGLOPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGLOInTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGCFWOutTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGEGTC1To3InTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGEGTC4To6InTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGEngineSpeed = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGEngineLoad = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGEngineRunHour = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGLOInPress = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGLOFilterInPress = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGControlAirPress = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGTCLOPress = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGEngineRunning = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGUTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGVTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGWTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGBTDTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGCyl1ExTemp = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGCyl2ExTemp = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGCyl3ExTemp = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGCyl4ExTemp = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGCyl5ExTemp = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGCyl6ExTemp = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGTCEXOutTemp = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGBoostAirPress = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DGFOInPress = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_maingeneratorset", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_mainswitchboard",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MBVoltageHigh = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MBVoltageLow = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MBFrequencyHigh = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MBFrequencyLow = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGRunning = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGPower = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGVoltageL1L2 = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGVoltageL2L3 = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGVoltageL3L1 = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGCurrentL1 = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGCurrentL2 = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGCurrentL3 = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGFrequency = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DGPowerFactor = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_mainswitchboard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_meremotecontrol",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MERpm = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_meremotecontrol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_miscellaneous",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MECCOMHigh = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    MEAxialVibration = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MELoad = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    METCSpeed = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_miscellaneous", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_scavengeair",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MEReceiverTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEFBCyl1Temp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEFBCyl2Temp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEFBCyl3Temp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEFBCyl4Temp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEFBCyl5Temp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEFBCyl6Temp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MEPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MECoolerPressDrop = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    METCInTempA = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    METCInTempB = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    ERRelativeHumidity = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    ERTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    ERAmbientPress = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_scavengeair", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "engineroom_shaftclutch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SternAftTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    InterTemp = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_shaftclutch", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UK_CompositeBoiler_NRD",
                table: "engineroom_compositeboiler",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_CompressedAirSupply_NRD",
                table: "engineroom_compressedairsupply",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_CoolingFreshWater_NRD",
                table: "engineroom_coolingfreshwater",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_CoolingSeaWater_NRD",
                table: "engineroom_coolingseawater",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_CoolingWater_NRD",
                table: "engineroom_coolingwater",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_CylinderLubOil_NRD",
                table: "engineroom_cylinderluboil",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_ExhaustGas_NRD",
                table: "engineroom_exhaustgas",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_FO_NRD",
                table: "engineroom_fo",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_FOSupplyUnit_NRD",
                table: "engineroom_fosupplyunit",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_LubOil_NRD",
                table: "engineroom_luboil",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_LubOilPurifying_NRD",
                table: "engineroom_luboilpurifying",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_MainGeneratorSet_NRD",
                table: "engineroom_maingeneratorset",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_MainSwitchboard_NRD",
                table: "engineroom_mainswitchboard",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_MERemoteControl_NRD",
                table: "engineroom_meremotecontrol",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_Miscellaneous_NRD",
                table: "engineroom_miscellaneous",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_ScavengeAir_NRD",
                table: "engineroom_scavengeair",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_ShaftClutch_NRD",
                table: "engineroom_shaftclutch",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "engineroom_compositeboiler");

            migrationBuilder.DropTable(
                name: "engineroom_compressedairsupply");

            migrationBuilder.DropTable(
                name: "engineroom_coolingfreshwater");

            migrationBuilder.DropTable(
                name: "engineroom_coolingseawater");

            migrationBuilder.DropTable(
                name: "engineroom_coolingwater");

            migrationBuilder.DropTable(
                name: "engineroom_cylinderluboil");

            migrationBuilder.DropTable(
                name: "engineroom_exhaustgas");

            migrationBuilder.DropTable(
                name: "engineroom_fo");

            migrationBuilder.DropTable(
                name: "engineroom_fosupplyunit");

            migrationBuilder.DropTable(
                name: "engineroom_luboil");

            migrationBuilder.DropTable(
                name: "engineroom_luboilpurifying");

            migrationBuilder.DropTable(
                name: "engineroom_maingeneratorset");

            migrationBuilder.DropTable(
                name: "engineroom_mainswitchboard");

            migrationBuilder.DropTable(
                name: "engineroom_meremotecontrol");

            migrationBuilder.DropTable(
                name: "engineroom_miscellaneous");

            migrationBuilder.DropTable(
                name: "engineroom_scavengeair");

            migrationBuilder.DropTable(
                name: "engineroom_shaftclutch");

            migrationBuilder.DropColumn(
                name: "SeaTemperature",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "Energy",
                table: "energy_shaft");

            migrationBuilder.DropColumn(
                name: "Revolutions",
                table: "energy_shaft");
        }
    }
}
