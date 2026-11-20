using CareService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CareService.Application.DTOs.Admin;
using CareService.Application.Interfaces.Services;
using CareBridge.Shared.Exceptions;

namespace CareService.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IAppointmentStatsClient _appointmentStatsClient;
        private readonly IUserStatsClient _userStatsClient;

        public AdminService(
            IAdminRepository adminRepository,
            IAppointmentStatsClient appointmentStatsClient,
            IUserStatsClient userStatsClient)
        {
            _adminRepository = adminRepository;
            _appointmentStatsClient = appointmentStatsClient;
            _userStatsClient = userStatsClient;
        }

        public async Task<ConsultationStatisticsDto> GetConsultationStatisticsAsync(DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken)
        {
            if (from.HasValue && to.HasValue && from > to)
            {
                throw new BadRequestException(
                    "From date cannot be greater than To date.");
            }

            return await _adminRepository
                .GetConsultationStatisticsAsync(
                    from,
                    to,
                    cancellationToken);
        }

        public async Task<AdminDashboardResponseDto> GetDashboardAsync(CancellationToken cancellationToken)
        {
            return await _adminRepository.GetDashboardAsync(
                cancellationToken);
        }

        public async Task<LabOrderStatisticsDto> GetLabOrderStatisticsAsync(string? status, CancellationToken cancellationToken)
        {
            return await _adminRepository.GetLabOrderStatisticsAsync(
               status,
               cancellationToken);
        }

        public async Task<EntityCountsDto> GetEntityCountsAsync(
            CancellationToken cancellationToken)
        {
            var counts = await _userStatsClient.GetCountsAsync(
                cancellationToken);

            return counts ?? new EntityCountsDto();
        }

        public async Task<AppointmentStatisticsDto> GetAppointmentStatisticsAsync(
            CancellationToken cancellationToken)
        {
            var statistics = await _appointmentStatsClient
                .GetStatisticsAsync(cancellationToken);

            return statistics ?? new AppointmentStatisticsDto();
        }

        public async Task<IReadOnlyList<AppointmentTrendDto>> GetAppointmentTrendsAsync(
            int? year,
            CancellationToken cancellationToken)
        {
            var trends = await _appointmentStatsClient
                .GetMonthlyTrendsAsync(year, cancellationToken);

            return trends?.ToList()
                ?? new List<AppointmentTrendDto>();
        }

        public async Task<IReadOnlyList<PopularDoctorDto>> GetPopularDoctorsAsync(
            int limit,
            CancellationToken cancellationToken)
        {
            if (limit < 1 || limit > 20)
            {
                limit = 5;
            }

            var popular = await _appointmentStatsClient
                .GetPopularDoctorsAsync(limit, cancellationToken);

            if (popular == null || popular.Count == 0)
            {
                return new List<PopularDoctorDto>();
            }

            var summaries = await _userStatsClient.GetDoctorSummariesAsync(
                popular.Select(x => x.DoctorId).ToList(),
                cancellationToken);

            var summariesById = (summaries ?? new List<DoctorSummaryDto>())
                .ToDictionary(x => x.UserId);

            return popular
                .Select(entry =>
                {
                    summariesById.TryGetValue(
                        entry.DoctorId,
                        out var summary);

                    var fullName = summary == null
                        ? "Unknown doctor"
                        : $"{summary.FirstName} {summary.LastName}".Trim();

                    return new PopularDoctorDto
                    {
                        DoctorId = entry.DoctorId,

                        FullName = fullName,

                        Specialty = string.IsNullOrWhiteSpace(
                            summary?.SpecializationName)
                                ? "General medicine"
                                : summary!.SpecializationName!,

                        AppointmentCount = entry.AppointmentCount,

                        ProfileImageUrl = summary?.ProfileImageUrl
                    };
                })
                .ToList();
        }
    }
}

