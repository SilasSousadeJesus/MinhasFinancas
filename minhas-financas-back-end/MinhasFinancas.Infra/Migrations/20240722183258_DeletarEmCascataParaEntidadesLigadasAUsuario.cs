using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class DeletarEmCascataParaEntidadesLigadasAUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banco_Users_UsuarioId",
                table: "Banco");

            migrationBuilder.DropForeignKey(
                name: "FK_Cartao_Users_UsuarioId",
                table: "Cartao");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamento_Users_UsuarioId",
                table: "Lancamento");

            migrationBuilder.AddForeignKey(
                name: "FK_Banco_Users_UsuarioId",
                table: "Banco",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cartao_Users_UsuarioId",
                table: "Cartao",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamento_Users_UsuarioId",
                table: "Lancamento",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banco_Users_UsuarioId",
                table: "Banco");

            migrationBuilder.DropForeignKey(
                name: "FK_Cartao_Users_UsuarioId",
                table: "Cartao");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamento_Users_UsuarioId",
                table: "Lancamento");

            migrationBuilder.AddForeignKey(
                name: "FK_Banco_Users_UsuarioId",
                table: "Banco",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cartao_Users_UsuarioId",
                table: "Cartao",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamento_Users_UsuarioId",
                table: "Lancamento",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
