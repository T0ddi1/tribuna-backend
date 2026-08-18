using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPortal.Api.Migrations
{
    /// <inheritdoc />
    public partial class VerticalECategoriaSeparadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artigos_Categorias_CategoriaId",
                table: "Artigos");

            migrationBuilder.DropColumn(
                name: "CorAccent",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "CorAccentDark",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "CorAccentTint",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Icone",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Ordem",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Tagline",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "TemaEscuro",
                table: "Categorias");

            migrationBuilder.AlterColumn<int>(
                name: "CategoriaId",
                table: "Artigos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "VerticalId",
                table: "Artigos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Verticais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    Tagline = table.Column<string>(type: "TEXT", nullable: true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    Icone = table.Column<string>(type: "TEXT", nullable: true),
                    CorAccent = table.Column<string>(type: "TEXT", nullable: true),
                    CorAccentDark = table.Column<string>(type: "TEXT", nullable: true),
                    CorAccentTint = table.Column<string>(type: "TEXT", nullable: true),
                    TemaEscuro = table.Column<bool>(type: "INTEGER", nullable: false),
                    Ordem = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verticais", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artigos_VerticalId",
                table: "Artigos",
                column: "VerticalId");

            migrationBuilder.CreateIndex(
                name: "IX_Verticais_Slug",
                table: "Verticais",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Artigos_Categorias_CategoriaId",
                table: "Artigos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Artigos_Verticais_VerticalId",
                table: "Artigos",
                column: "VerticalId",
                principalTable: "Verticais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artigos_Categorias_CategoriaId",
                table: "Artigos");

            migrationBuilder.DropForeignKey(
                name: "FK_Artigos_Verticais_VerticalId",
                table: "Artigos");

            migrationBuilder.DropTable(
                name: "Verticais");

            migrationBuilder.DropIndex(
                name: "IX_Artigos_VerticalId",
                table: "Artigos");

            migrationBuilder.DropColumn(
                name: "VerticalId",
                table: "Artigos");

            migrationBuilder.AddColumn<string>(
                name: "CorAccent",
                table: "Categorias",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorAccentDark",
                table: "Categorias",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorAccentTint",
                table: "Categorias",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Categorias",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icone",
                table: "Categorias",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Ordem",
                table: "Categorias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tagline",
                table: "Categorias",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TemaEscuro",
                table: "Categorias",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "CategoriaId",
                table: "Artigos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Artigos_Categorias_CategoriaId",
                table: "Artigos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
