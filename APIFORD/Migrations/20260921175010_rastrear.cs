using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class rastrear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "post_id",
                table: "atividades_workspace",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "ix_atividades_workspace_post_id",
                table: "atividades_workspace",
                column: "post_id");

            migrationBuilder.AddForeignKey(
                name: "fk_atividades_workspace_workspace_posts_post_id",
                table: "atividades_workspace",
                column: "post_id",
                principalTable: "workspace_posts",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_atividades_workspace_workspace_posts_post_id",
                table: "atividades_workspace");

            migrationBuilder.DropIndex(
                name: "ix_atividades_workspace_post_id",
                table: "atividades_workspace");

            migrationBuilder.AlterColumn<int>(
                name: "post_id",
                table: "atividades_workspace",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
