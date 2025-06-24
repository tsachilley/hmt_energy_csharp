using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.Migrations
{
    /// <inheritdoc />
    public partial class updatesentencevdridToSN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "grdspdknot",
                table: "vdr_vtg",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "grdspdkm",
                table: "vdr_vtg",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "grdcoztrue",
                table: "vdr_vtg",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "grdcozmag",
                table: "vdr_vtg",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "watdistotal",
                table: "vdr_vlw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "watdisreset",
                table: "vdr_vlw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "grddistotal",
                table: "vdr_vlw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "grddisreset",
                table: "vdr_vlw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "watspd",
                table: "vdr_vbw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "tvswatspdstern",
                table: "vdr_vbw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "tvswatspd",
                table: "vdr_vbw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "tvsgrdspdstern",
                table: "vdr_vbw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "tvsgrdspd",
                table: "vdr_vbw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "lngwatspd",
                table: "vdr_vbw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "lnggrdspd",
                table: "vdr_vbw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "grdspd",
                table: "vdr_vbw",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "speed",
                table: "vdr_rpm",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "propellerpitch",
                table: "vdr_rpm",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "speed",
                table: "vdr_mwv",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "angle",
                table: "vdr_mwv",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "tdirection",
                table: "vdr_mwd",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "speed",
                table: "vdr_mwd",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "magdirection",
                table: "vdr_mwd",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "knspeed",
                table: "vdr_mwd",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "offset",
                table: "vdr_dpt",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "mrs",
                table: "vdr_dpt",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "depth",
                table: "vdr_dpt",
                type: "float",
                maxLength: 32,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "vdr_id",
                table: "sentence",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "grdspdknot",
                table: "vdr_vtg",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "grdspdkm",
                table: "vdr_vtg",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "grdcoztrue",
                table: "vdr_vtg",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "grdcozmag",
                table: "vdr_vtg",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "watdistotal",
                table: "vdr_vlw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "watdisreset",
                table: "vdr_vlw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "grddistotal",
                table: "vdr_vlw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "grddisreset",
                table: "vdr_vlw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "watspd",
                table: "vdr_vbw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tvswatspdstern",
                table: "vdr_vbw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tvswatspd",
                table: "vdr_vbw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tvsgrdspdstern",
                table: "vdr_vbw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tvsgrdspd",
                table: "vdr_vbw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "lngwatspd",
                table: "vdr_vbw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "lnggrdspd",
                table: "vdr_vbw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "grdspd",
                table: "vdr_vbw",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "speed",
                table: "vdr_rpm",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "propellerpitch",
                table: "vdr_rpm",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "speed",
                table: "vdr_mwv",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "angle",
                table: "vdr_mwv",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tdirection",
                table: "vdr_mwd",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "speed",
                table: "vdr_mwd",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "magdirection",
                table: "vdr_mwd",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "knspeed",
                table: "vdr_mwd",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "offset",
                table: "vdr_dpt",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "mrs",
                table: "vdr_dpt",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "depth",
                table: "vdr_dpt",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "vdr_id",
                table: "sentence",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}