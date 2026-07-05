using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class patrimonio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Passivo",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Passivo",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Passivo",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Passivo",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "BemPatrimonial",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAquisicao",
                table: "BemPatrimonial",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SnapshotPatrimonial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DataReferencia = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TotalAtivos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPassivos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PatrimonioLiquido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataCriacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapshotPatrimonial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnapshotPatrimonial_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Passivo_UsuarioId",
                table: "Passivo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapshotPatrimonial_UsuarioId",
                table: "SnapshotPatrimonial",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Passivo_Users_UsuarioId",
                table: "Passivo",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passivo_Users_UsuarioId",
                table: "Passivo");

            migrationBuilder.DropTable(
                name: "SnapshotPatrimonial");

            migrationBuilder.DropIndex(
                name: "IX_Passivo_UsuarioId",
                table: "Passivo");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Passivo");

            migrationBuilder.DropColumn(
                name: "DataFim",
                table: "Passivo");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Passivo");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "BemPatrimonial");

            migrationBuilder.DropColumn(
                name: "DataAquisicao",
                table: "BemPatrimonial");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Passivo",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
