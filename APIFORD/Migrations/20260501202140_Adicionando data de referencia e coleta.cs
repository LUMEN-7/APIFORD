using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class Adicionandodatadereferenciaecoleta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataColeta",
                table: "Sources",
                newName: "AddAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataColeta",
                table: "TireSources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataReferencia",
                table: "TireSources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataColeta",
                table: "SpecificationSources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataReferencia",
                table: "SpecificationSources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataColeta",
                table: "ModeSources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataReferencia",
                table: "ModeSources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataColeta",
                table: "DimensionSources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataReferencia",
                table: "DimensionSources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataColeta",
                table: "ConsumeSources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataReferencia",
                table: "ConsumeSources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataColeta",
                table: "AdditionalSources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataReferencia",
                table: "AdditionalSources",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataColeta",
                table: "TireSources");

            migrationBuilder.DropColumn(
                name: "DataReferencia",
                table: "TireSources");

            migrationBuilder.DropColumn(
                name: "DataColeta",
                table: "SpecificationSources");

            migrationBuilder.DropColumn(
                name: "DataReferencia",
                table: "SpecificationSources");

            migrationBuilder.DropColumn(
                name: "DataColeta",
                table: "ModeSources");

            migrationBuilder.DropColumn(
                name: "DataReferencia",
                table: "ModeSources");

            migrationBuilder.DropColumn(
                name: "DataColeta",
                table: "DimensionSources");

            migrationBuilder.DropColumn(
                name: "DataReferencia",
                table: "DimensionSources");

            migrationBuilder.DropColumn(
                name: "DataColeta",
                table: "ConsumeSources");

            migrationBuilder.DropColumn(
                name: "DataReferencia",
                table: "ConsumeSources");

            migrationBuilder.DropColumn(
                name: "DataColeta",
                table: "AdditionalSources");

            migrationBuilder.DropColumn(
                name: "DataReferencia",
                table: "AdditionalSources");

            migrationBuilder.RenameColumn(
                name: "AddAt",
                table: "Sources",
                newName: "DataColeta");
        }
    }
}
