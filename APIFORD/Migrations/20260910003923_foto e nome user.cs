using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class fotoenomeuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "foto",
                table: "AspNetUsers",
                newName: "foto_perfil_url");

            migrationBuilder.AddColumn<string>(
                name: "nome_exibicao",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nome_exibicao",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "foto_perfil_url",
                table: "AspNetUsers",
                newName: "foto");
        }
    }
}
