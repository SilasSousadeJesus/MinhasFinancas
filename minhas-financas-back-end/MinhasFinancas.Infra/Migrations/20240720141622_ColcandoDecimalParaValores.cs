using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ColcandoDecimalParaValores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Valor",
                table: "LancamentoParcelado");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "LancamentoFixo");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Lancamento");

            migrationBuilder.DropColumn(
                name: "Limite",
                table: "Cartao");

            migrationBuilder.AlterColumn<int>(
                name: "NumeroParcela",
                table: "LancamentoParcelado",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "Saldo",
                table: "LancamentoParcelado",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Saldo",
                table: "LancamentoFixo",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Saldo",
                table: "Lancamento",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Saldo",
                table: "Cartao",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Saldo",
                table: "Banco",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Saldo",
                table: "LancamentoParcelado");

            migrationBuilder.DropColumn(
                name: "Saldo",
                table: "LancamentoFixo");

            migrationBuilder.DropColumn(
                name: "Saldo",
                table: "Lancamento");

            migrationBuilder.DropColumn(
                name: "Saldo",
                table: "Cartao");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroParcela",
                table: "LancamentoParcelado",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Valor",
                table: "LancamentoParcelado",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Valor",
                table: "LancamentoFixo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Valor",
                table: "Lancamento",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Limite",
                table: "Cartao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Saldo",
                table: "Banco",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
