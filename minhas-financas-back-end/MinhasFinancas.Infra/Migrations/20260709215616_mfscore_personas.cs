using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class mfscore_personas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonaMfScore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ObjetivoDaPersona = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RendaMensal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceitasPrevistas30Dias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceitasPrevistas90Dias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceitasPrevistas180Dias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceitasPrevistas12Meses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DespesasMensais = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Obrigacoes30Dias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Obrigacoes90Dias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Obrigacoes180Dias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Obrigacoes12Meses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReservaEmergencia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PatrimonioBruto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Passivos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PatrimonioLiquido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PossuiPerfilFinanceiroConfigurado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PossuiPlanoEstrategico = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PossuiMetas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PossuiCompromissos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CompromissosCumpridos = table.Column<int>(type: "int", nullable: false),
                    PossuiInadimplencia = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ScoreHumanoSugerido = table.Column<int>(type: "int", nullable: true),
                    FaixaEsperadaMin = table.Column<int>(type: "int", nullable: true),
                    FaixaEsperadaMax = table.Column<int>(type: "int", nullable: true),
                    JustificativaNotaHumana = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EhCasoCanonico = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Observacoes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataCriacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaMfScore", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonaMfScore");
        }
    }
}
