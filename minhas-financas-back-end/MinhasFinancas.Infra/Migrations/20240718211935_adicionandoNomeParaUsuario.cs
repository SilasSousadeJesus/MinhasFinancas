using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class adicionandoNomeParaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomeProprio",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeProprio",
                table: "AspNetUsers");
        }
    }
}
