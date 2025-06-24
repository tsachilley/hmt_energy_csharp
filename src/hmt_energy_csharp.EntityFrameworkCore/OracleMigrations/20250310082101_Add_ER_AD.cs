using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class AddERAD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "engineroom_assistantdecision",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Key = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Content = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    State = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Uploaded = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ReceiveDatetime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeviceNo = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineroom_assistantdecision", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UK_AssistantDecision_NRD",
                table: "engineroom_assistantdecision",
                columns: new[] { "Number", "ReceiveDatetime", "DeviceNo" },
                unique: true,
                filter: "\"Number\" IS NOT NULL AND \"DeviceNo\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "engineroom_assistantdecision");
        }
    }
}
