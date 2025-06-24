using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class addprediction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "energy_prediction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DGO = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: false),
                    LFO = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: false),
                    HFO = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: false),
                    LPGP = table.Column<decimal>(name: "LPG_P", type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: false),
                    LPGB = table.Column<decimal>(name: "LPG_B", type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: false),
                    LNG = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: false),
                    Methanol = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: false),
                    Ethanol = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_prediction", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "energy_prediction");
        }
    }
}
