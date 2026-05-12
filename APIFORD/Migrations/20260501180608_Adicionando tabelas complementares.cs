using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class Adicionandotabelascomplementares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sources_Carros_Carro_id",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Tires");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Specifications");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Dimensions");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Consumes");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Additionals");

            migrationBuilder.AlterColumn<int>(
                name: "Carro_id",
                table: "Sources",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "AdditionalSources",
                columns: table => new
                {
                    AdditionalId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalSources", x => new { x.AdditionalId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_AdditionalSources_Additionals_AdditionalId",
                        column: x => x.AdditionalId,
                        principalTable: "Additionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdditionalSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumeSources",
                columns: table => new
                {
                    ConsumeId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumeSources", x => new { x.ConsumeId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_ConsumeSources_Consumes_ConsumeId",
                        column: x => x.ConsumeId,
                        principalTable: "Consumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumeSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DimensionSources",
                columns: table => new
                {
                    DimensionId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionSources", x => new { x.DimensionId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_DimensionSources_Dimensions_DimensionId",
                        column: x => x.DimensionId,
                        principalTable: "Dimensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DimensionSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModeSources",
                columns: table => new
                {
                    ModeId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    ModeSourceModeId = table.Column<int>(type: "int", nullable: true),
                    ModeSourceSourceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeSources", x => new { x.ModeId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_ModeSources_ModeSources_ModeSourceModeId_ModeSourceSourceId",
                        columns: x => new { x.ModeSourceModeId, x.ModeSourceSourceId },
                        principalTable: "ModeSources",
                        principalColumns: new[] { "ModeId", "SourceId" });
                    table.ForeignKey(
                        name: "FK_ModeSources_Modes_ModeId",
                        column: x => x.ModeId,
                        principalTable: "Modes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModeSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedModels",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Carro_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedModels", x => new { x.UserId, x.Carro_id });
                    table.ForeignKey(
                        name: "FK_SavedModels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedModels_Carros_Carro_id",
                        column: x => x.Carro_id,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationSources",
                columns: table => new
                {
                    SpecificationId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationSources", x => new { x.SpecificationId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_SpecificationSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecificationSources_Specifications_SpecificationId",
                        column: x => x.SpecificationId,
                        principalTable: "Specifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TireSources",
                columns: table => new
                {
                    TireId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TireSources", x => new { x.TireId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_TireSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TireSources_Tires_TireId",
                        column: x => x.TireId,
                        principalTable: "Tires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalSources_SourceId",
                table: "AdditionalSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumeSources_SourceId",
                table: "ConsumeSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionSources_SourceId",
                table: "DimensionSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeSources_ModeSourceModeId_ModeSourceSourceId",
                table: "ModeSources",
                columns: new[] { "ModeSourceModeId", "ModeSourceSourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ModeSources_SourceId",
                table: "ModeSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedModels_Carro_id",
                table: "SavedModels",
                column: "Carro_id");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationSources_SourceId",
                table: "SpecificationSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_TireSources_SourceId",
                table: "TireSources",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sources_Carros_Carro_id",
                table: "Sources",
                column: "Carro_id",
                principalTable: "Carros",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sources_Carros_Carro_id",
                table: "Sources");

            migrationBuilder.DropTable(
                name: "AdditionalSources");

            migrationBuilder.DropTable(
                name: "ConsumeSources");

            migrationBuilder.DropTable(
                name: "DimensionSources");

            migrationBuilder.DropTable(
                name: "ModeSources");

            migrationBuilder.DropTable(
                name: "SavedModels");

            migrationBuilder.DropTable(
                name: "SpecificationSources");

            migrationBuilder.DropTable(
                name: "TireSources");

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Tires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Specifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Carro_id",
                table: "Sources",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Dimensions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Consumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Additionals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Sources_Carros_Carro_id",
                table: "Sources",
                column: "Carro_id",
                principalTable: "Carros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
