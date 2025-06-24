using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class updatevesselinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sid",
                table: "vesselinfo");

            migrationBuilder.AlterColumn<string>(
                name: "Weather",
                table: "vesselinfo",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BLGHFOCACC",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BLGMDOCACC",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BLRFCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BLRHFOCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BLRMDOCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DGFCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DGHFOCACC",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DGHFOCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DGMDOCACC",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DGMDOCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DGPower",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HFOCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MDOCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MEFCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MEHFOCACC",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MEHFOCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MEMDOCACC",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MEMDOCPerNm",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SFOC",
                table: "vesselinfo",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SN",
                table: "vesselinfo",
                type: "NVARCHAR2(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BLGHFOCACC",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "BLGMDOCACC",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "BLRFCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "BLRHFOCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "BLRMDOCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "DGFCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "DGHFOCACC",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "DGHFOCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "DGMDOCACC",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "DGMDOCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "DGPower",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "FCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "HFOCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "MDOCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "MEFCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "MEHFOCACC",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "MEHFOCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "MEMDOCACC",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "MEMDOCPerNm",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "SFOC",
                table: "vesselinfo");

            migrationBuilder.DropColumn(
                name: "SN",
                table: "vesselinfo");

            migrationBuilder.AlterColumn<string>(
                name: "Weather",
                table: "vesselinfo",
                type: "NVARCHAR2(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sid",
                table: "vesselinfo",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);
        }
    }
}