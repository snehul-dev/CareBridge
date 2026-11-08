using CareBridge.Shared.Responses;
using CareService.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using CareBridge.Contracts.Labs;

namespace CareService.Infrastructure.ExternalServices
{
    public class LabDirectoryClient : ILabDirectoryClient
    {
        private readonly HttpClient _httpClient;

        public LabDirectoryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> IsVerifiedLabAsync(Guid labId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                $"api/labs/{labId}/verification",
                cancellationToken
                );


            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            var apiResponse =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<LabVerificationResponseDto>>(
                    cancellationToken: cancellationToken);

            return apiResponse?.Data?.IsVerified == true;

        }
    }
}
