using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class notificacoespart2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "linhagem_id_referenciado",
                table: "notificacoes_eventos",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "linhagem_id_referenciado",
                table: "notificacoes_eventos");
        }
    }
}
