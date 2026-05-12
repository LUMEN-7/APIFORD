using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class RemovendoShadowPropertyCarro_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sources_Carros_Carro_id",
                table: "Sources");

            migrationBuilder.DropIndex(
                name: "IX_Sources_Carro_id",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "Carro_id",
                table: "Sources");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Carro_id",
                table: "Sources",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Carro_id",
                table: "Sources",
                column: "Carro_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sources_Carros_Carro_id",
                table: "Sources",
                column: "Carro_id",
                principalTable: "Carros",
                principalColumn: "Id");
        }
    }
}
