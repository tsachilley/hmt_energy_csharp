using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class addtotalindicator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Thrust",
                table: "energy_shaft",
                type: "DECIMAL(7,2)",
                precision: 7,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(6,2)",
                oldPrecision: 6,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "energy_totalindicator",
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
                    Power = table.Column<decimal>(type: "DECIMAL(7,2)", precision: 7, scale: 2, nullable: false),
                    Torque = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    Thrust = table.Column<decimal>(type: "DECIMAL(7,2)", precision: 7, scale: 2, nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_totalindicator", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "energy_totalindicator");

            migrationBuilder.AlterColumn<decimal>(
                name: "Thrust",
                table: "energy_shaft",
                type: "DECIMAL(6,2)",
                precision: 6,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
