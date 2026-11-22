using CareBridge.Shared.Responses;
using CareService.Application.DTOs.Admin;
using CareService.Application.Interfaces.Services;
using System.Net;
using System.Net.Http.Json;

namespace CareService.Infrastructure.ExternalServices
{
    public class AppointmentStatsClient : IAppointmentStatsClient
    {
        private readonly HttpClient _httpClient;

        public AppointmentStatsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AppointmentStatisticsDto?> GetStatisticsAsync(
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                "api/appointments/internal/appointments/stats",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var apiResponse =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<AppointmentStatisticsDto>>(
                    cancellationToken: cancellationToken);

            return apiResponse?.Data;
        }

        public async Task<IReadOnlyList<AppointmentTrendDto>?> GetMonthlyTrendsAsync(
            int? year,
            CancellationToken cancellationToken)
        {
            var url = year.HasValue
                ? $"api/appointments/internal/appointments/trends?year={year.Value}"
                : "api/appointments/internal/appointments/trends";

            var response = await _httpClient.GetAsync(
                url,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var apiResponse =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<List<AppointmentTrendDto>>>(
                    cancellationToken: cancellationToken);

            return apiResponse?.Data;
        }

        public async Task<IReadOnlyList<DoctorAppointmentCountDto>?> GetPopularDoctorsAsync(
            int limit,
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                $"api/appointments/internal/appointments/popular-doctors?limit={limit}",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var apiResponse =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<List<DoctorAppointmentCountDto>>>(
                    cancellationToken: cancellationToken);

            return apiResponse?.Data;
        }
    }
}
