using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPortal.Api.Migrations
{
    /// <inheritdoc />
    public partial class AutorExibicao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AutorExibicao",
                table: "Artigos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutorExibicao",
                table: "Artigos");
        }
    }
}
