using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RefatoraStatusEDataLancamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataPagamento",
                table: "Lancamento",
                newName: "DataVencimento");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEfetivacao",
                table: "Lancamento",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusLancamento",
                table: "Lancamento",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE Lancamento
                SET
                    StatusLancamento = CASE
                        WHEN Realizado = 0 THEN 0
                        WHEN Tipo IN (1, 3, 6) THEN 2
                        ELSE 1
                    END,
                    DataEfetivacao = CASE
                        WHEN Realizado = 1 THEN DataVencimento
                        ELSE NULL
                    END
                """);

            migrationBuilder.DropColumn(
                name: "Realizado",
                table: "Lancamento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataEfetivacao",
                table: "Lancamento");

            migrationBuilder.DropColumn(
                name: "StatusLancamento",
                table: "Lancamento");

            migrationBuilder.RenameColumn(
                name: "DataVencimento",
                table: "Lancamento",
                newName: "DataPagamento");

            migrationBuilder.AddColumn<bool>(
                name: "Realizado",
                table: "Lancamento",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
