using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLabResultPdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportFileUrl",
                table: "LabResults");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "LabResults",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "LabResults",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfFileName",
                table: "LabResults",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfStorageKey",
                table: "LabResults",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UploadedAt",
                table: "LabResults",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UploadedBy",
                table: "LabResults",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "LabResults");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "LabResults");

            migrationBuilder.DropColumn(
                name: "PdfFileName",
                table: "LabResults");

            migrationBuilder.DropColumn(
                name: "PdfStorageKey",
                table: "LabResults");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "LabResults");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "LabResults");

            migrationBuilder.AddColumn<string>(
                name: "ReportFileUrl",
                table: "LabResults",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
