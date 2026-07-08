using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class compromisso_analise_vinculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AnaliseFinanceiraHistoricaId",
                table: "CompromissoFinanceiro",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "CompromissoFinanceiroId",
                table: "AnaliseFinanceiraHistorica",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnaliseFinanceiraHistoricaId",
                table: "CompromissoFinanceiro");

            migrationBuilder.DropColumn(
                name: "CompromissoFinanceiroId",
                table: "AnaliseFinanceiraHistorica");
        }
    }
}
