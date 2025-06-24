using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class AddPowerUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "energy_powerunit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DGO = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true, comment: "柴油瞬时消耗"),
                    LFO = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true, comment: "低硫重油瞬时消耗"),
                    HFO = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true, comment: "重油瞬时消耗"),
                    LPGP = table.Column<decimal>(name: "LPG_P", type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true, comment: "液化石油气(丙烷)瞬时消耗"),
                    LPGB = table.Column<decimal>(name: "LPG_B", type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true, comment: "液化石油气(丁烷)瞬时消耗"),
                    LNG = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true, comment: "液化天然气瞬时消耗"),
                    Methanol = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true, comment: "甲醇瞬时消耗"),
                    Ethanol = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true, comment: "乙醇瞬时消耗"),
                    DGOAccumulated = table.Column<decimal>(type: "DECIMAL(14,4)", precision: 14, scale: 4, nullable: true, comment: "柴油累积消耗"),
                    LFOAccumulated = table.Column<decimal>(type: "DECIMAL(14,4)", precision: 14, scale: 4, nullable: true, comment: "低硫重油累积消耗"),
                    HFOAccumulated = table.Column<decimal>(type: "DECIMAL(14,4)", precision: 14, scale: 4, nullable: true, comment: "重油累积消耗"),
                    LPGPAccumulated = table.Column<decimal>(name: "LPG_PAccumulated", type: "DECIMAL(14,4)", precision: 14, scale: 4, nullable: true, comment: "液化石油气(丙烷)累积消耗"),
                    LPGBAccumulated = table.Column<decimal>(name: "LPG_BAccumulated", type: "DECIMAL(14,4)", precision: 14, scale: 4, nullable: true, comment: "液化石油气(丁烷)累积消耗"),
                    LNGAccumulated = table.Column<decimal>(type: "DECIMAL(14,4)", precision: 14, scale: 4, nullable: true, comment: "液化天然气累积消耗"),
                    MethanolAccumulated = table.Column<decimal>(type: "DECIMAL(14,4)", precision: 14, scale: 4, nullable: true, comment: "甲醇累积消耗"),
                    EthanolAccumulated = table.Column<decimal>(type: "DECIMAL(14,4)", precision: 14, scale: 4, nullable: true, comment: "乙醇累积消耗"),
                    DeviceType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true, comment: "动力单元类型:me:主机 ae:辅机 blr:锅炉"),
                    Number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_powerunit", x => x.Id);
                },
                comment: "动力单元能耗");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "energy_powerunit");
        }
    }
}
