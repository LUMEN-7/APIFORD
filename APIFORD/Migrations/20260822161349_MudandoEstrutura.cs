using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class MudandoEstrutura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "carro_modos");

            migrationBuilder.DropTable(
                name: "modos");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "jobs",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "categoria",
                table: "carros",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "modos",
                table: "carros",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categoria",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "modos",
                table: "carros");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "jobs",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "modos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "carro_modos",
                columns: table => new
                {
                    carro_id = table.Column<int>(type: "integer", nullable: false),
                    modo_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_carro_modos", x => new { x.carro_id, x.modo_id });
                    table.ForeignKey(
                        name: "fk_carro_modos_carros_carro_id",
                        column: x => x.carro_id,
                        principalTable: "carros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_carro_modos_modos_modo_id",
                        column: x => x.modo_id,
                        principalTable: "modos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_carro_modos_modo_id",
                table: "carro_modos",
                column: "modo_id");
        }
    }
}
