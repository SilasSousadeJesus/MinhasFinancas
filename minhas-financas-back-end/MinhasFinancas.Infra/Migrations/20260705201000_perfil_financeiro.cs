using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    public partial class perfil_financeiro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerfilFinanceiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UsuarioId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataCriacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilFinanceiro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilFinanceiro_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConfiguracaoPerfilFinanceiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PerfilFinanceiroId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DataInicioVigencia = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataFimVigencia = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PercentualEconomiaMensalDesejado = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PercentualReservaEmergenciaDesejado = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MesesReservaEmergenciaDesejados = table.Column<int>(type: "int", nullable: false),
                    PercentualMaximoComprometimentoRenda = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PercentualMaximoEndividamento = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PercentualMinimoInvestimento = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PatrimonioLiquidoAlvo = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Observacao = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataCriacao = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoPerfilFinanceiro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracaoPerfilFinanceiro_PerfilFinanceiro_PerfilFinancei~",
                        column: x => x.PerfilFinanceiroId,
                        principalTable: "PerfilFinanceiro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracaoPerfilFinanceiro_PerfilFinanceiroId",
                table: "ConfiguracaoPerfilFinanceiro",
                column: "PerfilFinanceiroId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilFinanceiro_UsuarioId",
                table: "PerfilFinanceiro",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracaoPerfilFinanceiro");

            migrationBuilder.DropTable(
                name: "PerfilFinanceiro");
        }
    }
}
