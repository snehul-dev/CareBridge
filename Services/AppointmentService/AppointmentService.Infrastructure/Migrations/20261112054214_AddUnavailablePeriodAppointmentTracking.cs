using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnavailablePeriodAppointmentTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        ALTER TABLE "DoctorAvailabilities"
        ALTER COLUMN "AvailableDate" TYPE date
        USING "AvailableDate"::date;
        """);

            migrationBuilder.AddColumn<string>(
                name: "StatusBeforeUnavailable",
                table: "Appointments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnavailablePeriodId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UnavailablePeriodId",
                table: "Appointments",
                column: "UnavailablePeriodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_UnavailablePeriodId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "StatusBeforeUnavailable",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "UnavailablePeriodId",
                table: "Appointments");

            migrationBuilder.Sql("""
        ALTER TABLE "DoctorAvailabilities"
        ALTER COLUMN "AvailableDate" TYPE character varying(10)
        USING "AvailableDate"::varchar(10);
        """);
        }
    }
}
