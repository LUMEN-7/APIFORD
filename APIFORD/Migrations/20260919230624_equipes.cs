using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class equipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "equipe_id",
                table: "workspace_posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "equipes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    criador_user_id = table.Column<string>(type: "text", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipe_membros",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    equipe_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    papel = table.Column<string>(type: "text", nullable: false),
                    entrou_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipe_membros", x => x.id);
                    table.ForeignKey(
                        name: "fk_equipe_membros_equipes_equipe_id",
                        column: x => x.equipe_id,
                        principalTable: "equipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_workspace_posts_equipe_id",
                table: "workspace_posts",
                column: "equipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipe_membros_equipe_id_user_id",
                table: "equipe_membros",
                columns: new[] { "equipe_id", "user_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_posts_equipes_equipe_id",
                table: "workspace_posts",
                column: "equipe_id",
                principalTable: "equipes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_workspace_posts_equipes_equipe_id",
                table: "workspace_posts");

            migrationBuilder.DropTable(
                name: "equipe_membros");

            migrationBuilder.DropTable(
                name: "equipes");

            migrationBuilder.DropIndex(
                name: "ix_workspace_posts_equipe_id",
                table: "workspace_posts");

            migrationBuilder.DropColumn(
                name: "equipe_id",
                table: "workspace_posts");
        }
    }
}
