using AppointmentService.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Interfaces
{
    public interface IAppointmentStatsRepository
    {
        Task<AppointmentStatisticsDto> GetStatisticsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<AppointmentTrendDto>> GetMonthlyTrendsAsync(
            int year,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<PopularDoctorAppointmentDto>> GetPopularDoctorsAsync(
            int limit,
            CancellationToken cancellationToken);
    }
}
