using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class projecao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projecao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataInicial = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorAcumuladoInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorObjetivo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MesesLimite = table.Column<int>(type: "int", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projecao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projecao_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RendaProjecao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorMensal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjecaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RendaProjecao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RendaProjecao_Projecao_ProjecaoId",
                        column: x => x.ProjecaoId,
                        principalTable: "Projecao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projecao_UsuarioId",
                table: "Projecao",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RendaProjecao_ProjecaoId",
                table: "RendaProjecao",
                column: "ProjecaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RendaProjecao");

            migrationBuilder.DropTable(
                name: "Projecao");
        }
    }
}
