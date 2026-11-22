using CareBridge.Shared.Responses;
using CareService.Application.Interfaces.Services;
using System.Net;
using System.Net.Http.Json;

namespace CareService.Infrastructure.ExternalServices
{
    public class InternalLabClient : IInternalLabClient
    {
        private readonly HttpClient _httpClient;

        public InternalLabClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetLabNameAsync(
            Guid labId,
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                $"api/users/internal/labs/{labId}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content
                .ReadFromJsonAsync<ApiResponse<LabNameDto>>(
                    cancellationToken: cancellationToken);

            return apiResponse?.Data?.LabName;
        }

        private sealed class LabNameDto
        {
            public Guid Id { get; set; }
            public string LabName { get; set; } = string.Empty;
            public Guid UserId { get; set; }
        }
    }
}
