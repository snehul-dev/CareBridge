
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace AppointmentService.Infrastructure.Clients
{
    public class AuthorizationHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationHeaderHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var authorizationHeader =
                _httpContextAccessor.HttpContext?
                    .Request.Headers.Authorization.ToString();

            if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
                AuthenticationHeaderValue.TryParse(
                    authorizationHeader, out var header))
            {
                request.Headers.Authorization = header;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
