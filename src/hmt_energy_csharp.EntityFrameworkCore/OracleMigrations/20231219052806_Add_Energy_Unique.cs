using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class AddEnergyUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_totalindicator",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_supplyunit",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_supplyunit",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_sternsealing",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_sternsealing",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_shaft",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_shaft",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_prediction",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_powerunit",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceType",
                table: "energy_powerunit",
                type: "NVARCHAR2(450)",
                nullable: true,
                comment: "动力单元类型:me:主机 ae:辅机 blr:锅炉",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true,
                oldComment: "动力单元类型:me:主机 ae:辅机 blr:锅炉");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_liquidlevel",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_liquidlevel",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_generator",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_generator",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_flowmeter",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_flowmeter",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_battery",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_battery",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UK_TotalIndicator_NR",
                table: "energy_totalindicator",
                columns: new[] { "Number", "ReceiveDatetime" },
                unique: true,
                filter: "\"Number\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_SupplyUnit_NRD",
                table: "energy_supplyunit",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_SternSealing_NRD",
                table: "energy_sternsealing",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_Shaft_NRD",
                table: "energy_shaft",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_Prediction_NR",
                table: "energy_prediction",
                columns: new[] { "Number", "ReceiveDatetime" },
                unique: true,
                filter: "\"Number\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_PowerUnit_DNR",
                table: "energy_powerunit",
                columns: new[] { "DeviceType", "Number", "ReceiveDatetime" },
                unique: true,
                filter: "\"DeviceType\" IS NOT NULL AND \"Number\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_LiquidLevel_NRD",
                table: "energy_liquidlevel",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_Generator_NRD",
                table: "energy_generator",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_Flowmeter_NRD",
                table: "energy_flowmeter",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UK_Battery_NRD",
                table: "energy_battery",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_TotalIndicator_NR",
                table: "energy_totalindicator");

            migrationBuilder.DropIndex(
                name: "UK_SupplyUnit_NRD",
                table: "energy_supplyunit");

            migrationBuilder.DropIndex(
                name: "UK_SternSealing_NRD",
                table: "energy_sternsealing");

            migrationBuilder.DropIndex(
                name: "UK_Shaft_NRD",
                table: "energy_shaft");

            migrationBuilder.DropIndex(
                name: "UK_Prediction_NR",
                table: "energy_prediction");

            migrationBuilder.DropIndex(
                name: "UK_PowerUnit_DNR",
                table: "energy_powerunit");

            migrationBuilder.DropIndex(
                name: "UK_LiquidLevel_NRD",
                table: "energy_liquidlevel");

            migrationBuilder.DropIndex(
                name: "UK_Generator_NRD",
                table: "energy_generator");

            migrationBuilder.DropIndex(
                name: "UK_Flowmeter_NRD",
                table: "energy_flowmeter");

            migrationBuilder.DropIndex(
                name: "UK_Battery_NRD",
                table: "energy_battery");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_totalindicator",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_supplyunit",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_supplyunit",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_sternsealing",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_sternsealing",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_shaft",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_shaft",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_prediction",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_powerunit",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceType",
                table: "energy_powerunit",
                type: "NVARCHAR2(2000)",
                nullable: true,
                comment: "动力单元类型:me:主机 ae:辅机 blr:锅炉",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true,
                oldComment: "动力单元类型:me:主机 ae:辅机 blr:锅炉");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_liquidlevel",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_liquidlevel",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_generator",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_generator",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_flowmeter",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_flowmeter",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "energy_battery",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "energy_battery",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);
        }
    }
}
