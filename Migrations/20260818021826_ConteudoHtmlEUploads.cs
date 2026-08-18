using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPortal.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConteudoHtmlEUploads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConteudoJson",
                table: "Artigos",
                newName: "ConteudoHtml");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConteudoHtml",
                table: "Artigos",
                newName: "ConteudoJson");
        }
    }
}
