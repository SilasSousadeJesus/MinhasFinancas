using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class reajustandoBemPatrimonial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Valor",
                table: "BemPatrimonial");

            migrationBuilder.AddColumn<bool>(
                name: "Permanencia",
                table: "BemPatrimonial",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PermanenciaBemMaterial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataPermanencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BemPatrimonialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermanenciaBemMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermanenciaBemMaterial_BemPatrimonial_BemPatrimonialId",
                        column: x => x.BemPatrimonialId,
                        principalTable: "BemPatrimonial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermanenciaBemMaterial_BemPatrimonialId",
                table: "PermanenciaBemMaterial",
                column: "BemPatrimonialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermanenciaBemMaterial");

            migrationBuilder.DropColumn(
                name: "Permanencia",
                table: "BemPatrimonial");

            migrationBuilder.AddColumn<decimal>(
                name: "Valor",
                table: "BemPatrimonial",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
