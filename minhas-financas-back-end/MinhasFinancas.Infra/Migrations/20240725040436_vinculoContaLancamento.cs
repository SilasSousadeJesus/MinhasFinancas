using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class vinculoContaLancamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContaId",
                table: "Lancamento",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoInvestimento",
                table: "Conta",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_ContaId",
                table: "Lancamento",
                column: "ContaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamento_Conta_ContaId",
                table: "Lancamento",
                column: "ContaId",
                principalTable: "Conta",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lancamento_Conta_ContaId",
                table: "Lancamento");

            migrationBuilder.DropIndex(
                name: "IX_Lancamento_ContaId",
                table: "Lancamento");

            migrationBuilder.DropColumn(
                name: "ContaId",
                table: "Lancamento");

            migrationBuilder.DropColumn(
                name: "SaldoInvestimento",
                table: "Conta");
        }
    }
}
