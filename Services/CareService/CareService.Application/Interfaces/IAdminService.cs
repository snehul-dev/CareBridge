using System;
using System.Collections.Generic;
using System.Text;
using CareService.Application.DTOs.Admin;

namespace CareService.Application.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardResponseDto> GetDashboardAsync(
           CancellationToken cancellationToken);

        Task<ConsultationStatisticsDto> GetConsultationStatisticsAsync(
            DateTimeOffset? from,
            DateTimeOffset? to,
            CancellationToken cancellationToken);

        Task<LabOrderStatisticsDto> GetLabOrderStatisticsAsync(
            string? status,
            CancellationToken cancellationToken);

        Task<EntityCountsDto> GetEntityCountsAsync(
            CancellationToken cancellationToken);

        Task<AppointmentStatisticsDto> GetAppointmentStatisticsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<AppointmentTrendDto>> GetAppointmentTrendsAsync(
            int? year,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<PopularDoctorDto>> GetPopularDoctorsAsync(
            int limit,
            CancellationToken cancellationToken);
    }
}
