using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentConcurrencyProtection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        CREATE EXTENSION IF NOT EXISTS btree_gist;

        ALTER TABLE "Appointments"
        ADD CONSTRAINT "EX_Appointments_Doctor_Time_NoOverlap"
        EXCLUDE USING gist
        (
            "DoctorId" WITH =,
            tsrange(
                "AppointmentDate" + "StartTime",
                "AppointmentDate" + "EndTime",
                '[)'
            ) WITH &&
        )
        WHERE ("Status" IN ('Pending', 'Confirmed'));
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
    ALTER TABLE "Appointments"
    DROP CONSTRAINT IF EXISTS
        "EX_Appointments_Doctor_Time_NoOverlap";
    """);

        }
    }
}
