using CareService.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces.Services
{
    public interface IAppointmentStatsClient
    {
        Task<AppointmentStatisticsDto?> GetStatisticsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<AppointmentTrendDto>?> GetMonthlyTrendsAsync(
            int? year,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<DoctorAppointmentCountDto>?> GetPopularDoctorsAsync(
            int limit,
            CancellationToken cancellationToken);
    }
}
