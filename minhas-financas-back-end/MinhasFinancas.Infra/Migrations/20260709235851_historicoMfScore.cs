using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class historicoMfScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricoMfScore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UsuarioId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompetenciaAno = table.Column<int>(type: "int", nullable: false),
                    CompetenciaMes = table.Column<int>(type: "int", nullable: false),
                    MfScoreBase = table.Column<int>(type: "int", nullable: false),
                    MfScoreFinal = table.Column<int>(type: "int", nullable: false),
                    Classificacao = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Risco = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PenalidadeTotal = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    DataCalculo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    VersaoModelo = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JsonPilares = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JsonIndicadoresCriticos = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JsonResumo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoMfScore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoMfScore_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoMfScore_CompetenciaAno_CompetenciaMes",
                table: "HistoricoMfScore",
                columns: new[] { "CompetenciaAno", "CompetenciaMes" });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoMfScore_UsuarioId_CompetenciaAno_CompetenciaMes_Ver~",
                table: "HistoricoMfScore",
                columns: new[] { "UsuarioId", "CompetenciaAno", "CompetenciaMes", "VersaoModelo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoMfScore");
        }
    }
}
