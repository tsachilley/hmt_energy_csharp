using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class addupdatetotaladdacc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "energy_totalindicator",
                comment: "多传感器累积量");

            migrationBuilder.AlterColumn<decimal>(
                name: "Torque",
                table: "energy_totalindicator",
                type: "DECIMAL(6,2)",
                precision: 6,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(6,2)",
                oldPrecision: 6,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Thrust",
                table: "energy_totalindicator",
                type: "DECIMAL(7,2)",
                precision: 7,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Power",
                table: "energy_totalindicator",
                type: "DECIMAL(7,2)",
                precision: 7,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Methanol",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "LPG_P",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "LPG_B",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "LNG",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "LFO",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "HFO",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Ethanol",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "DGO",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AddColumn<decimal>(
                name: "DGOAccumulated",
                table: "energy_totalindicator",
                type: "DECIMAL(14,4)",
                precision: 14,
                scale: 4,
                nullable: true,
                comment: "柴油累积消耗");

            migrationBuilder.AddColumn<decimal>(
                name: "EthanolAccumulated",
                table: "energy_totalindicator",
                type: "DECIMAL(14,4)",
                precision: 14,
                scale: 4,
                nullable: true,
                comment: "乙醇累积消耗");

            migrationBuilder.AddColumn<decimal>(
                name: "HFOAccumulated",
                table: "energy_totalindicator",
                type: "DECIMAL(14,4)",
                precision: 14,
                scale: 4,
                nullable: true,
                comment: "重油累积消耗");

            migrationBuilder.AddColumn<decimal>(
                name: "LFOAccumulated",
                table: "energy_totalindicator",
                type: "DECIMAL(14,4)",
                precision: 14,
                scale: 4,
                nullable: true,
                comment: "低硫重油累积消耗");

            migrationBuilder.AddColumn<decimal>(
                name: "LNGAccumulated",
                table: "energy_totalindicator",
                type: "DECIMAL(14,4)",
                precision: 14,
                scale: 4,
                nullable: true,
                comment: "液化天然气累积消耗");

            migrationBuilder.AddColumn<decimal>(
                name: "LPG_BAccumulated",
                table: "energy_totalindicator",
                type: "DECIMAL(14,4)",
                precision: 14,
                scale: 4,
                nullable: true,
                comment: "液化石油气(丁烷)累积消耗");

            migrationBuilder.AddColumn<decimal>(
                name: "LPG_PAccumulated",
                table: "energy_totalindicator",
                type: "DECIMAL(14,4)",
                precision: 14,
                scale: 4,
                nullable: true,
                comment: "液化石油气(丙烷)累积消耗");

            migrationBuilder.AddColumn<decimal>(
                name: "MethanolAccumulated",
                table: "energy_totalindicator",
                type: "DECIMAL(14,4)",
                precision: 14,
                scale: 4,
                nullable: true,
                comment: "甲醇累积消耗");

            migrationBuilder.AlterColumn<decimal>(
                name: "Methanol",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "LPG_P",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "LPG_B",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "LNG",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "LFO",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "HFO",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Ethanol",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                comment: "乙醇",
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "DGO",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DGOAccumulated",
                table: "energy_totalindicator");

            migrationBuilder.DropColumn(
                name: "EthanolAccumulated",
                table: "energy_totalindicator");

            migrationBuilder.DropColumn(
                name: "HFOAccumulated",
                table: "energy_totalindicator");

            migrationBuilder.DropColumn(
                name: "LFOAccumulated",
                table: "energy_totalindicator");

            migrationBuilder.DropColumn(
                name: "LNGAccumulated",
                table: "energy_totalindicator");

            migrationBuilder.DropColumn(
                name: "LPG_BAccumulated",
                table: "energy_totalindicator");

            migrationBuilder.DropColumn(
                name: "LPG_PAccumulated",
                table: "energy_totalindicator");

            migrationBuilder.DropColumn(
                name: "MethanolAccumulated",
                table: "energy_totalindicator");

            migrationBuilder.AlterTable(
                name: "energy_totalindicator",
                oldComment: "多传感器累积量");

            migrationBuilder.AlterColumn<decimal>(
                name: "Torque",
                table: "energy_totalindicator",
                type: "DECIMAL(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(6,2)",
                oldPrecision: 6,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Thrust",
                table: "energy_totalindicator",
                type: "DECIMAL(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Power",
                table: "energy_totalindicator",
                type: "DECIMAL(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Methanol",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LPG_P",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LPG_B",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LNG",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LFO",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "HFO",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Ethanol",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DGO",
                table: "energy_totalindicator",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Methanol",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LPG_P",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LPG_B",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LNG",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LFO",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "HFO",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Ethanol",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true,
                oldComment: "乙醇");

            migrationBuilder.AlterColumn<decimal>(
                name: "DGO",
                table: "energy_prediction",
                type: "DECIMAL(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);
        }
    }
}
