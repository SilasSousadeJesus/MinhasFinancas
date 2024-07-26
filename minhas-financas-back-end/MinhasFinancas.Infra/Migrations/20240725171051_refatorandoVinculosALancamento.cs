using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class refatorandoVinculosALancamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentificaoOrigem",
                table: "Lancamento");

            migrationBuilder.RenameColumn(
                name: "Origem",
                table: "Lancamento",
                newName: "Vinculo");

            migrationBuilder.AddColumn<Guid>(
                name: "CartaoId",
                table: "Lancamento",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_CartaoId",
                table: "Lancamento",
                column: "CartaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamento_Cartao_CartaoId",
                table: "Lancamento",
                column: "CartaoId",
                principalTable: "Cartao",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lancamento_Cartao_CartaoId",
                table: "Lancamento");

            migrationBuilder.DropIndex(
                name: "IX_Lancamento_CartaoId",
                table: "Lancamento");

            migrationBuilder.DropColumn(
                name: "CartaoId",
                table: "Lancamento");

            migrationBuilder.RenameColumn(
                name: "Vinculo",
                table: "Lancamento",
                newName: "Origem");

            migrationBuilder.AddColumn<Guid>(
                name: "IdentificaoOrigem",
                table: "Lancamento",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
