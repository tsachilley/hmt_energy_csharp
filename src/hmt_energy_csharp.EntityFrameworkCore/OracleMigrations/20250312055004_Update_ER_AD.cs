using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class UpdateERAD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_AssistantDecision_NRD",
                table: "engineroom_assistantdecision");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "engineroom_assistantdecision",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "engineroom_assistantdecision",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UK_AssistantDecision_NRK",
                table: "engineroom_assistantdecision",
                columns: new[] { "Number", "ReceiveDatetime", "Key" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"Key\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_AssistantDecision_NRK",
                table: "engineroom_assistantdecision");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "engineroom_assistantdecision",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceNo",
                table: "engineroom_assistantdecision",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UK_AssistantDecision_NRD",
                table: "engineroom_assistantdecision",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");
        }
    }
}
