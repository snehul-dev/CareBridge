
using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using CareBridge.Shared.Responses;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppointmentService.Infrastructure.Clients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web);

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DoctorSearchResponse?> GetDoctorByIdAsync(
            Guid doctorId,
            CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(
                $"api/doctors/{doctorId}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<
                ApiResponse<DoctorSearchResponse>>(
                    JsonOptions,
                    cancellationToken);

            return result?.Data;
        }

        public async Task<IReadOnlyList<DoctorSearchResponse>> SearchDoctorsAsync(
            string? specialization,
            string? city,
            string? district,
            CancellationToken cancellationToken)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                query.Add(
                    $"specialization={Uri.EscapeDataString(specialization.Trim())}");
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query.Add(
                    $"city={Uri.EscapeDataString(city.Trim())}");
            }

            if (!string.IsNullOrWhiteSpace(district))
            {
                query.Add(
                    $"district={Uri.EscapeDataString(district.Trim())}");
            }

            var url = "api/doctors";

            if (query.Count > 0)
                url += "?" + string.Join("&", query);

            using var response = await _httpClient.GetAsync(
                url,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<
                ApiResponse<IReadOnlyList<DoctorSearchResponse>>>(
                    JsonOptions,
                    cancellationToken);

            if (result?.Data is null)
            {
                throw new HttpRequestException(
                    "UserService returned an empty doctor search response.");
            }

            return result.Data;
        }
    }
}
