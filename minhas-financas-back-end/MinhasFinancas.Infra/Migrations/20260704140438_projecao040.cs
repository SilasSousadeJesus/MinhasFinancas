using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class projecao040 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GrupoLancamentoProgramadoId",
                table: "Lancamento",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumeroDiaUtil",
                table: "Lancamento",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoProgramacao",
                table: "Lancamento",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrupoLancamentoProgramadoId",
                table: "Lancamento");

            migrationBuilder.DropColumn(
                name: "NumeroDiaUtil",
                table: "Lancamento");

            migrationBuilder.DropColumn(
                name: "TipoProgramacao",
                table: "Lancamento");
        }
    }
}
