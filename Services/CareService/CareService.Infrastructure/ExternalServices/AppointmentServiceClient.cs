using CareBridge.Contracts.Appointments;
using CareBridge.Shared.Responses;
using CareService.Application.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace CareService.Infrastructure.ExternalServices
{
    public class AppointmentServiceClient : IAppointmentServiceClient
    {
        private readonly HttpClient _httpClient;

        public AppointmentServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(
            Guid appointmentId,
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                $"api/appointments/{appointmentId}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var apiResponse =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<AppointmentDetailsDto>>(
                    cancellationToken: cancellationToken);

            return apiResponse?.Data;
        }
    }
}