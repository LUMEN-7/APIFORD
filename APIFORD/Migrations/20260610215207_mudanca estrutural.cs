using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class mudancaestrutural : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consumo_fontes");

            migrationBuilder.DropTable(
                name: "dimensao_fontes");

            migrationBuilder.DropTable(
                name: "especificacao_fontes");

            migrationBuilder.DropTable(
                name: "extra_fontes");

            migrationBuilder.DropTable(
                name: "modo_fontes");

            migrationBuilder.DropTable(
                name: "pneu_fontes");

            migrationBuilder.DropColumn(
                name: "rpm_potencia",
                table: "especificacoes");

            migrationBuilder.DropColumn(
                name: "rpm_torque",
                table: "especificacoes");

            migrationBuilder.AlterColumn<string>(
                name: "perfil",
                table: "pneus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "largura",
                table: "pneus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "aro",
                table: "pneus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_coleta",
                table: "pneus",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "url",
                table: "fontes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_coleta",
                table: "extras",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "torque",
                table: "especificacoes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "potencia",
                table: "especificacoes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_coleta",
                table: "especificacoes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "potencia_rpm",
                table: "especificacoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "torque_rpm",
                table: "especificacoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AlterColumn<string>(
                name: "largura",
                table: "dimensoes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "entre_eixos",
                table: "dimensoes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "comprimento",
                table: "dimensoes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "altura",
                table: "dimensoes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_coleta",
                table: "dimensoes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "data_coleta",
                table: "consumos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_coleta",
                table: "pneus");

            migrationBuilder.DropColumn(
                name: "url",
                table: "fontes");

            migrationBuilder.DropColumn(
                name: "data_coleta",
                table: "extras");

            migrationBuilder.DropColumn(
                name: "data_coleta",
                table: "especificacoes");

            migrationBuilder.DropColumn(
                name: "potencia_rpm",
                table: "especificacoes");

            migrationBuilder.DropColumn(
                name: "torque_rpm",
                table: "especificacoes");

            migrationBuilder.DropColumn(
                name: "data_coleta",
                table: "dimensoes");

            migrationBuilder.DropColumn(
                name: "data_coleta",
                table: "consumos");

            migrationBuilder.AlterColumn<int>(
                name: "perfil",
                table: "pneus",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "largura",
                table: "pneus",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "aro",
                table: "pneus",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "torque",
                table: "especificacoes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "potencia",
                table: "especificacoes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "rpm_potencia",
                table: "especificacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rpm_torque",
                table: "especificacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "largura",
                table: "dimensoes",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "entre_eixos",
                table: "dimensoes",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "comprimento",
                table: "dimensoes",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "altura",
                table: "dimensoes",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "consumo_fontes",
                columns: table => new
                {
                    consumo_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consumo_fontes", x => new { x.consumo_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_consumo_fontes_consumos_consumo_id",
                        column: x => x.consumo_id,
                        principalTable: "consumos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_consumo_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dimensao_fontes",
                columns: table => new
                {
                    dimensao_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dimensao_fontes", x => new { x.dimensao_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_dimensao_fontes_dimensoes_dimensao_id",
                        column: x => x.dimensao_id,
                        principalTable: "dimensoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dimensao_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "especificacao_fontes",
                columns: table => new
                {
                    especificacao_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_especificacao_fontes", x => new { x.especificacao_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_especificacao_fontes_especificacoes_especificacao_id",
                        column: x => x.especificacao_id,
                        principalTable: "especificacoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_especificacao_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "extra_fontes",
                columns: table => new
                {
                    extra_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_extra_fontes", x => new { x.extra_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_extra_fontes_extras_extra_id",
                        column: x => x.extra_id,
                        principalTable: "extras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_extra_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "modo_fontes",
                columns: table => new
                {
                    modo_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modo_fonte_fonte_id = table.Column<int>(type: "int", nullable: true),
                    modo_fonte_modo_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modo_fontes", x => new { x.modo_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_modo_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_modo_fontes_modo_fontes_modo_fonte_modo_id_modo_fonte_fonte_id",
                        columns: x => new { x.modo_fonte_modo_id, x.modo_fonte_fonte_id },
                        principalTable: "modo_fontes",
                        principalColumns: new[] { "modo_id", "fonte_id" });
                    table.ForeignKey(
                        name: "fk_modo_fontes_modos_modo_id",
                        column: x => x.modo_id,
                        principalTable: "modos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pneu_fontes",
                columns: table => new
                {
                    pneu_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pneu_fontes", x => new { x.pneu_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_pneu_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pneu_fontes_pneus_pneu_id",
                        column: x => x.pneu_id,
                        principalTable: "pneus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_consumo_fontes_fonte_id",
                table: "consumo_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_dimensao_fontes_fonte_id",
                table: "dimensao_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_especificacao_fontes_fonte_id",
                table: "especificacao_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_extra_fontes_fonte_id",
                table: "extra_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_modo_fontes_fonte_id",
                table: "modo_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_modo_fontes_modo_fonte_modo_id_modo_fonte_fonte_id",
                table: "modo_fontes",
                columns: new[] { "modo_fonte_modo_id", "modo_fonte_fonte_id" });

            migrationBuilder.CreateIndex(
                name: "ix_pneu_fontes_fonte_id",
                table: "pneu_fontes",
                column: "fonte_id");
        }
    }
}
