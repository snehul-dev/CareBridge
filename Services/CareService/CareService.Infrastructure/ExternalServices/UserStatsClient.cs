using CareBridge.Shared.Responses;
using CareService.Application.DTOs.Admin;
using CareService.Application.Interfaces.Services;
using System.Linq;
using System.Net.Http.Json;

namespace CareService.Infrastructure.ExternalServices
{
    public class UserStatsClient : IUserStatsClient
    {
        private readonly HttpClient _httpClient;

        public UserStatsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EntityCountsDto?> GetCountsAsync(
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                "api/users/internal/users/counts",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var apiResponse =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<EntityCountsDto>>(
                    cancellationToken: cancellationToken);

            return apiResponse?.Data;
        }

        public async Task<IReadOnlyList<DoctorSummaryDto>?> GetDoctorSummariesAsync(
            IReadOnlyCollection<Guid> userIds,
            CancellationToken cancellationToken)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return Array.Empty<DoctorSummaryDto>();
            }

            var query = string.Join(
                "&",
                userIds.Select(id => $"ids={id}"));

            var response = await _httpClient.GetAsync(
                $"api/users/internal/users/doctor-summaries?{query}",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var apiResponse =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<List<DoctorSummaryDto>>>(
                    cancellationToken: cancellationToken);

            return apiResponse?.Data;
        }
    }
}
