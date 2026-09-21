using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class equipecode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "codigo_convite",
                table: "equipes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "descricao",
                table: "equipes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_equipes_codigo_convite",
                table: "equipes",
                column: "codigo_convite",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_equipes_codigo_convite",
                table: "equipes");

            migrationBuilder.DropColumn(
                name: "codigo_convite",
                table: "equipes");

            migrationBuilder.DropColumn(
                name: "descricao",
                table: "equipes");
        }
    }
}
