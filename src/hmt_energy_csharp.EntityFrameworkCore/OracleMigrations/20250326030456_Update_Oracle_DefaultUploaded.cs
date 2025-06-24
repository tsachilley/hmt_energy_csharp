using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.OracleMigrations
{
    /// <inheritdoc />
    public partial class UpdateOracleDefaultUploaded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_shaftclutch",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_scavengeair",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_miscellaneous",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_meremotecontrol",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_mainswitchboard",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_maingeneratorset",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_luboilpurifying",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_luboil",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_fosupplyunit",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_fo",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_exhaustgas",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_cylinderluboil",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_coolingwater",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_coolingseawater",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_coolingfreshwater",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_compressedairsupply",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_compositeboiler",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_assistantdecision",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_shaftclutch",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_scavengeair",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_miscellaneous",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_meremotecontrol",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_mainswitchboard",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_maingeneratorset",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_luboilpurifying",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_luboil",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_fosupplyunit",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_fo",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_exhaustgas",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_cylinderluboil",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_coolingwater",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_coolingseawater",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_coolingfreshwater",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_compressedairsupply",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_compositeboiler",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "Uploaded",
                table: "engineroom_assistantdecision",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);
        }
    }
}
