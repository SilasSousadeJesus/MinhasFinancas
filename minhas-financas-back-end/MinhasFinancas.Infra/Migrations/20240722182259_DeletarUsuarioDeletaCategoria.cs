using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class DeletarUsuarioDeletaCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categoria_Users_UsuarioId",
                table: "Categoria");

            migrationBuilder.AddForeignKey(
                name: "FK_Categoria_Users_UsuarioId",
                table: "Categoria",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categoria_Users_UsuarioId",
                table: "Categoria");

            migrationBuilder.AddForeignKey(
                name: "FK_Categoria_Users_UsuarioId",
                table: "Categoria",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
