using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CorrigindoNomeValor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "LancamentoParcelado",
                newName: "Valor");

            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "LancamentoFixo",
                newName: "Valor");

            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "Lancamento",
                newName: "Valor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "LancamentoParcelado",
                newName: "Saldo");

            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "LancamentoFixo",
                newName: "Saldo");

            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "Lancamento",
                newName: "Saldo");
        }
    }
}
