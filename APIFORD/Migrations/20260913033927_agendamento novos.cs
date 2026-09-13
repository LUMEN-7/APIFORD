using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class agendamentonovos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "linhagem_id",
                table: "agendamentos_pesquisa",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notas",
                table: "agendamentos_pesquisa",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "linhagem_id",
                table: "agendamentos_pesquisa");

            migrationBuilder.DropColumn(
                name: "notas",
                table: "agendamentos_pesquisa");
        }
    }
}
