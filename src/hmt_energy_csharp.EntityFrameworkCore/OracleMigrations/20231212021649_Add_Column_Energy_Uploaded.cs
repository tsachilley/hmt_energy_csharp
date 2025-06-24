using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class AddColumnEnergyUploaded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "vesselinfo",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_totalindicator",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_supplyunit",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_sternsealing",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_shaft",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_prediction",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_powerunit",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_liquidlevel",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_generator",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_flowmeter",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");

            migrationBuilder.AddColumn<byte>(
                name: "Uploaded",
                table: "energy_battery",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                comment: "是否已上传");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_totalindicator");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_supplyunit");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_sternsealing");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_shaft");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_prediction");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_powerunit");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_liquidlevel");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_generator");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_flowmeter");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "energy_battery");
        }
    }
}
