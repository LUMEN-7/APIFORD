using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class workspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workspace_posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    autor_user_id = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    conteudo = table.Column<string>(type: "text", nullable: false),
                    tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    responsavel_user_id = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    tipo_conteudo_vinculado = table.Column<string>(type: "text", nullable: true),
                    conteudo_vinculado_id = table.Column<int>(type: "integer", nullable: true),
                    conteudo_vinculado_titulo = table.Column<string>(type: "text", nullable: true),
                    fixado = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workspace_posts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workspace_comentarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workspace_post_id = table.Column<int>(type: "integer", nullable: false),
                    autor_user_id = table.Column<string>(type: "text", nullable: false),
                    conteudo = table.Column<string>(type: "text", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workspace_comentarios", x => x.id);
                    table.ForeignKey(
                        name: "fk_workspace_comentarios_workspace_posts_workspace_post_id",
                        column: x => x.workspace_post_id,
                        principalTable: "workspace_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workspace_curtidas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workspace_post_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workspace_curtidas", x => x.id);
                    table.ForeignKey(
                        name: "fk_workspace_curtidas_workspace_posts_workspace_post_id",
                        column: x => x.workspace_post_id,
                        principalTable: "workspace_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_workspace_comentarios_workspace_post_id",
                table: "workspace_comentarios",
                column: "workspace_post_id");

            migrationBuilder.CreateIndex(
                name: "ix_workspace_curtidas_workspace_post_id_user_id",
                table: "workspace_curtidas",
                columns: new[] { "workspace_post_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workspace_comentarios");

            migrationBuilder.DropTable(
                name: "workspace_curtidas");

            migrationBuilder.DropTable(
                name: "workspace_posts");
        }
    }
}
