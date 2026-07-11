using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class base_simulacao_mf_score : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoCenarioSimulacao",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataGeracaoBaseSimulacao",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescricaoCenarioSimulacao",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "EhUsuarioSintetico",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ObjetivoCenarioSimulacao",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OrigemUsuario",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VersaoBaseSimulacao",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoCenarioSimulacao",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DataGeracaoBaseSimulacao",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DescricaoCenarioSimulacao",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EhUsuarioSintetico",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ObjetivoCenarioSimulacao",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OrigemUsuario",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VersaoBaseSimulacao",
                table: "Users");
        }
    }
}
