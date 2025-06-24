using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class UpdateOracleExhaustGas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MECyl1AfterTempDev",
                table: "engineroom_exhaustgas",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MECyl2AfterTempDev",
                table: "engineroom_exhaustgas",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MECyl3AfterTempDev",
                table: "engineroom_exhaustgas",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MECyl4AfterTempDev",
                table: "engineroom_exhaustgas",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MECyl5AfterTempDev",
                table: "engineroom_exhaustgas",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MECyl6AfterTempDev",
                table: "engineroom_exhaustgas",
                type: "BINARY_DOUBLE",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MECyl1AfterTempDev",
                table: "engineroom_exhaustgas");

            migrationBuilder.DropColumn(
                name: "MECyl2AfterTempDev",
                table: "engineroom_exhaustgas");

            migrationBuilder.DropColumn(
                name: "MECyl3AfterTempDev",
                table: "engineroom_exhaustgas");

            migrationBuilder.DropColumn(
                name: "MECyl4AfterTempDev",
                table: "engineroom_exhaustgas");

            migrationBuilder.DropColumn(
                name: "MECyl5AfterTempDev",
                table: "engineroom_exhaustgas");

            migrationBuilder.DropColumn(
                name: "MECyl6AfterTempDev",
                table: "engineroom_exhaustgas");
        }
    }
}
