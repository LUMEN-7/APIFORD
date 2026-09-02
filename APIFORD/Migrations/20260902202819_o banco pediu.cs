using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class obancopediu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "jobs",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_criacao",
                table: "carros",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "linhagem_id",
                table: "carros",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "versao_anterior_id",
                table: "carros",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_criacao",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "linhagem_id",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "versao_anterior_id",
                table: "carros");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "jobs",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");
        }
    }
}
