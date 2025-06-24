using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class AddConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "energy_config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Code = table.Column<string>(type: "varchar2(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "nvarchar2(30)", maxLength: 30, nullable: true),
                    Interval = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    HighLimit = table.Column<decimal>(type: "DECIMAL(12,4)", precision: 12, scale: 4, nullable: true),
                    HighHighLimit = table.Column<decimal>(type: "DECIMAL(12,4)", precision: 12, scale: 4, nullable: true),
                    IsDevice = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    IsEnabled = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "varchar2(30)", maxLength: 30, nullable: true),
                    createtime = table.Column<DateTime>(name: "create_time", type: "TIMESTAMP(7)", nullable: true),
                    updatetime = table.Column<DateTime>(name: "update_time", type: "TIMESTAMP(7)", nullable: true),
                    deletetime = table.Column<DateTime>(name: "delete_time", type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_config", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "energy_config");
        }
    }
}
