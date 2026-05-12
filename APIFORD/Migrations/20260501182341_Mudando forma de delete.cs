using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class Mudandoformadedelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalSources_Sources_SourceId",
                table: "AdditionalSources");

            migrationBuilder.DropForeignKey(
                name: "FK_CarroModes_Modes_ModeId",
                table: "CarroModes");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumeSources_Sources_SourceId",
                table: "ConsumeSources");

            migrationBuilder.DropForeignKey(
                name: "FK_DimensionSources_Sources_SourceId",
                table: "DimensionSources");

            migrationBuilder.DropForeignKey(
                name: "FK_ModeSources_Sources_SourceId",
                table: "ModeSources");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationSources_Sources_SourceId",
                table: "SpecificationSources");

            migrationBuilder.DropForeignKey(
                name: "FK_TireSources_Sources_SourceId",
                table: "TireSources");

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalSources_Sources_SourceId",
                table: "AdditionalSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarroModes_Modes_ModeId",
                table: "CarroModes",
                column: "ModeId",
                principalTable: "Modes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumeSources_Sources_SourceId",
                table: "ConsumeSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionSources_Sources_SourceId",
                table: "DimensionSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModeSources_Sources_SourceId",
                table: "ModeSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationSources_Sources_SourceId",
                table: "SpecificationSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TireSources_Sources_SourceId",
                table: "TireSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalSources_Sources_SourceId",
                table: "AdditionalSources");

            migrationBuilder.DropForeignKey(
                name: "FK_CarroModes_Modes_ModeId",
                table: "CarroModes");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumeSources_Sources_SourceId",
                table: "ConsumeSources");

            migrationBuilder.DropForeignKey(
                name: "FK_DimensionSources_Sources_SourceId",
                table: "DimensionSources");

            migrationBuilder.DropForeignKey(
                name: "FK_ModeSources_Sources_SourceId",
                table: "ModeSources");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationSources_Sources_SourceId",
                table: "SpecificationSources");

            migrationBuilder.DropForeignKey(
                name: "FK_TireSources_Sources_SourceId",
                table: "TireSources");

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalSources_Sources_SourceId",
                table: "AdditionalSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarroModes_Modes_ModeId",
                table: "CarroModes",
                column: "ModeId",
                principalTable: "Modes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumeSources_Sources_SourceId",
                table: "ConsumeSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionSources_Sources_SourceId",
                table: "DimensionSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModeSources_Sources_SourceId",
                table: "ModeSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationSources_Sources_SourceId",
                table: "SpecificationSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TireSources_Sources_SourceId",
                table: "TireSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
