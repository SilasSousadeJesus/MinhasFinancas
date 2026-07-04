using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class projecao03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AtreladaADespesas",
                table: "Projecao",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DividaManualProjecaoMensal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MesReferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjecaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DividaManualProjecaoMensal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DividaManualProjecaoMensal_Projecao_ProjecaoId",
                        column: x => x.ProjecaoId,
                        principalTable: "Projecao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DividaManualProjecaoMensal_ProjecaoId",
                table: "DividaManualProjecaoMensal",
                column: "ProjecaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DividaManualProjecaoMensal");

            migrationBuilder.DropColumn(
                name: "AtreladaADespesas",
                table: "Projecao");
        }
    }
}
