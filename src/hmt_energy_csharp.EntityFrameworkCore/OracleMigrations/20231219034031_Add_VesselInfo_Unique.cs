using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class AddVesselInfoUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UK_VesselInfo_RS",
                table: "vesselinfo",
                columns: new[] { "ReceiveDatetime", "SN" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_VesselInfo_RS",
                table: "vesselinfo");
        }
    }
}
