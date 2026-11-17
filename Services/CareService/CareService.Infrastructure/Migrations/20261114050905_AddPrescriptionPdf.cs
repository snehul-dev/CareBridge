using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionPdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfFileUrl",
                table: "Prescriptions");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PdfGeneratedAt",
                table: "Prescriptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfStorageKey",
                table: "Prescriptions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfGeneratedAt",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "PdfStorageKey",
                table: "Prescriptions");

            migrationBuilder.AddColumn<string>(
                name: "PdfFileUrl",
                table: "Prescriptions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
