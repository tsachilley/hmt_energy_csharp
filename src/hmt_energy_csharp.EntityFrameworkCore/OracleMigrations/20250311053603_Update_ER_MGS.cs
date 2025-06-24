using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class UpdateERMGS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "DGFOInPress",
                table: "engineroom_maingeneratorset",
                type: "BINARY_DOUBLE",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DGBoostAirPress",
                table: "engineroom_maingeneratorset",
                type: "BINARY_DOUBLE",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DGFOInPress",
                table: "engineroom_maingeneratorset",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "BINARY_DOUBLE",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DGBoostAirPress",
                table: "engineroom_maingeneratorset",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "BINARY_DOUBLE",
                oldNullable: true);
        }
    }
}
