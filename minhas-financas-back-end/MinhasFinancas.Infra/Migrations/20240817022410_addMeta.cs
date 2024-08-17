using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addMeta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeMeta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorAtual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorParaChegarNaMeta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PorcentagemDaMeta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetaAlcancada = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meta_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AporteMeta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataAporte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MetaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AporteMeta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AporteMeta_Meta_MetaId",
                        column: x => x.MetaId,
                        principalTable: "Meta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AporteMeta_MetaId",
                table: "AporteMeta",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Meta_UsuarioId",
                table: "Meta",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AporteMeta");

            migrationBuilder.DropTable(
                name: "Meta");
        }
    }
}
