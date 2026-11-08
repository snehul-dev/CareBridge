using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class userentitiesChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockReason",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BlockedBy",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "LabProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                table: "LabProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "LabProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VerifiedBy",
                table: "LabProfiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DoctorProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                table: "DoctorProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "DoctorProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VerifiedBy",
                table: "DoctorProfiles",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BlockedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BlockedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "LabProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "LabProfiles");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "LabProfiles");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "LabProfiles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "DoctorProfiles");
        }
    }
}
