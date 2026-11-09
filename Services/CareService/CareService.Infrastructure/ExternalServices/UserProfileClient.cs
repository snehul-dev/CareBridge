using CareBridge.Contracts.Users;
using CareBridge.Shared.Responses;
using CareService.Application.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace CareService.Infrastructure.ExternalServices
{
    public class UserProfileClient : IUserProfileClient
    {
        private readonly HttpClient _httpClient;

        public UserProfileClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserPrescriptionProfileDto?> GetPrescriptionProfileAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                $"api/internal/users/{userId}/prescription-profile",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var apiResponse =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<UserPrescriptionProfileDto>>(
                    cancellationToken: cancellationToken);

            return apiResponse?.Data;
        }
    }
}
