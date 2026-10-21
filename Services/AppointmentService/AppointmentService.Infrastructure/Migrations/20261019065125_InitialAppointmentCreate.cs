using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialAppointmentCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    AppointmentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.CheckConstraint("CK_Appointments_TimeRange", "\"EndTime\" > \"StartTime\"");
                });

            migrationBuilder.CreateTable(
                name: "DoctorAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorAvailabilities", x => x.Id);
                    table.CheckConstraint("CK_DoctorAvailabilities_TimeRange", "\"EndTime\" > \"StartTime\"");
                });

            migrationBuilder.CreateTable(
                name: "DoctorUnavailablePeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorUnavailablePeriods", x => x.Id);
                    table.CheckConstraint("CK_DoctorUnavailablePeriods_DateRange", "\"EndDate\" >= \"StartDate\"");
                });

            migrationBuilder.CreateTable(
                name: "ReferralRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromDoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToDoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralRequests_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId_AppointmentDate_Status",
                table: "Appointments",
                columns: new[] { "DoctorId", "AppointmentDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId_AppointmentDate",
                table: "Appointments",
                columns: new[] { "PatientId", "AppointmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAvailabilities_DoctorId_DayOfWeek",
                table: "DoctorAvailabilities",
                columns: new[] { "DoctorId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorUnavailablePeriods_DoctorId_StartDate_EndDate",
                table: "DoctorUnavailablePeriods",
                columns: new[] { "DoctorId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRequests_AppointmentId",
                table: "ReferralRequests",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRequests_ToDoctorId_Status",
                table: "ReferralRequests",
                columns: new[] { "ToDoctorId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorAvailabilities");

            migrationBuilder.DropTable(
                name: "DoctorUnavailablePeriods");

            migrationBuilder.DropTable(
                name: "ReferralRequests");

            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}
