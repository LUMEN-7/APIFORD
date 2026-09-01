using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class Salvamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "data_salvo",
                table: "modelo_salvos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ComparacoesSalvas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data_salvamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    request_payload = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comparacoes_salvas", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_comparacoes_salvas_user_id",
                table: "ComparacoesSalvas",
                column: "user_id");

            migrationBuilder.Sql("CREATE INDEX ix_carros_especificacoes_gin ON carros USING gin (especificacoes);");
            migrationBuilder.Sql("CREATE INDEX ix_carros_consumos_gin ON carros USING gin (consumos);");
            migrationBuilder.Sql("CREATE INDEX ix_carros_dimensoes_gin ON carros USING gin (dimensoes);");
            migrationBuilder.Sql("CREATE INDEX ix_carros_extras_gin ON carros USING gin (extras);");
            migrationBuilder.Sql("CREATE INDEX ix_carros_categoria_gin ON carros USING gin (categoria);");
            migrationBuilder.Sql("CREATE INDEX ix_carros_modos_gin ON carros USING gin (modos);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_carros_especificacoes_gin;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_carros_consumos_gin;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_carros_dimensoes_gin;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_carros_extras_gin;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_carros_categoria_gin;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_carros_modos_gin;");

            migrationBuilder.DropTable(
                name: "ComparacoesSalvas");

            migrationBuilder.DropColumn(
                name: "data_salvo",
                table: "modelo_salvos");
        }
    }
}
