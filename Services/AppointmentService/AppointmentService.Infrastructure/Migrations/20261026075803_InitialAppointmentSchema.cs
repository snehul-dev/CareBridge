using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialAppointmentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DayOfWeek",
                table: "DoctorAvailabilities",
                newName: "AvailableDate");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorAvailabilities_DoctorId_DayOfWeek",
                table: "DoctorAvailabilities",
                newName: "IX_DoctorAvailabilities_DoctorId_AvailableDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableDate",
                table: "DoctorAvailabilities",
                newName: "DayOfWeek");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorAvailabilities_DoctorId_AvailableDate",
                table: "DoctorAvailabilities",
                newName: "IX_DoctorAvailabilities_DoctorId_DayOfWeek");
        }
    }
}
