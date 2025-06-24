using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class addeeindexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "energy_ciicoefficient",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ShipType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Coefficient1 = table.Column<decimal>(type: "DECIMAL(16,0)", precision: 16, scale: 0, nullable: true),
                    Coefficient2 = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: true),
                    WeightCondition = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    WeightValue = table.Column<decimal>(type: "DECIMAL(6,0)", precision: 6, scale: 0, nullable: true),
                    LowValue = table.Column<decimal>(type: "DECIMAL(6,0)", precision: 6, scale: 0, nullable: true),
                    ContainLow = table.Column<decimal>(type: "DECIMAL(2,0)", precision: 2, scale: 0, nullable: true),
                    HighValue = table.Column<decimal>(type: "DECIMAL(6,0)", precision: 6, scale: 0, nullable: true),
                    ContainHigh = table.Column<decimal>(type: "DECIMAL(2,0)", precision: 2, scale: 0, nullable: true),
                    Sort = table.Column<decimal>(type: "DECIMAL(4,0)", precision: 4, scale: 0, nullable: true),
                    createtime = table.Column<DateTime>(name: "create_time", type: "TIMESTAMP(7)", nullable: true),
                    updatetime = table.Column<DateTime>(name: "update_time", type: "TIMESTAMP(7)", nullable: true),
                    deletetime = table.Column<DateTime>(name: "delete_time", type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_ciicoefficient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "energy_ciirating",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ShipType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Rating = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RatingValue = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: true),
                    WeightCondition = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    LowValue = table.Column<decimal>(type: "DECIMAL(6,0)", precision: 6, scale: 0, nullable: true),
                    ContainLow = table.Column<decimal>(type: "DECIMAL(2,0)", precision: 2, scale: 0, nullable: true),
                    HighValue = table.Column<decimal>(type: "DECIMAL(6,0)", precision: 6, scale: 0, nullable: true),
                    ContainHigh = table.Column<decimal>(type: "DECIMAL(2,0)", precision: 2, scale: 0, nullable: true),
                    Sort = table.Column<decimal>(type: "DECIMAL(4,0)", precision: 4, scale: 0, nullable: true),
                    createtime = table.Column<DateTime>(name: "create_time", type: "TIMESTAMP(7)", nullable: true),
                    updatetime = table.Column<DateTime>(name: "update_time", type: "TIMESTAMP(7)", nullable: true),
                    deletetime = table.Column<DateTime>(name: "delete_time", type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_ciirating", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "energy_fuelcoefficient",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Code = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NameCN = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NameEN = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Value = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: true),
                    Sort = table.Column<decimal>(type: "DECIMAL(4,0)", precision: 4, scale: 0, nullable: true),
                    createtime = table.Column<DateTime>(name: "create_time", type: "TIMESTAMP(7)", nullable: true),
                    updatetime = table.Column<DateTime>(name: "update_time", type: "TIMESTAMP(7)", nullable: true),
                    deletetime = table.Column<DateTime>(name: "delete_time", type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_fuelcoefficient", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "energy_ciicoefficient");

            migrationBuilder.DropTable(
                name: "energy_ciirating");

            migrationBuilder.DropTable(
                name: "energy_fuelcoefficient");
        }
    }
}
