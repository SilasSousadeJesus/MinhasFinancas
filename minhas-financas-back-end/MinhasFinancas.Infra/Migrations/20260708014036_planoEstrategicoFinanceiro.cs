using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class planoEstrategicoFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanoEstrategicoFinanceiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanoRaizId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UsuarioId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observacao = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroVersao = table.Column<int>(type: "int", nullable: false),
                    DataInicioVigencia = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataFimVigencia = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanoEstrategicoFinanceiro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanoEstrategicoFinanceiro_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ObjetivoPlanoEstrategico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanoEstrategicoFinanceiroId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prioridade = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    DataAlvo = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ValorAlvo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValorAtual = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Observacao = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataCriacao = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjetivoPlanoEstrategico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjetivoPlanoEstrategico_PlanoEstrategicoFinanceiro_PlanoEst~",
                        column: x => x.PlanoEstrategicoFinanceiroId,
                        principalTable: "PlanoEstrategicoFinanceiro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ObjetivoPlanoEstrategico_PlanoEstrategicoFinanceiroId_Ordem",
                table: "ObjetivoPlanoEstrategico",
                columns: new[] { "PlanoEstrategicoFinanceiroId", "Ordem" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanoEstrategicoFinanceiro_UsuarioId_Ativo",
                table: "PlanoEstrategicoFinanceiro",
                columns: new[] { "UsuarioId", "Ativo" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanoEstrategicoFinanceiro_UsuarioId_PlanoRaizId_NumeroVersao",
                table: "PlanoEstrategicoFinanceiro",
                columns: new[] { "UsuarioId", "PlanoRaizId", "NumeroVersao" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjetivoPlanoEstrategico");

            migrationBuilder.DropTable(
                name: "PlanoEstrategicoFinanceiro");
        }
    }
}
