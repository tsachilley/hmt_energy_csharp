using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmtenergycsharp.Migrations
{
    /// <inheritdoc />
    public partial class AddSentenceUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UK_Sentence_TVC",
                table: "sentence",
                columns: new[] { "time", "vdr_id", "category" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_Sentence_TVC",
                table: "sentence");
        }
    }
}
