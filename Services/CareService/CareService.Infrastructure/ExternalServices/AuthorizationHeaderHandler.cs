using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace CareService.Infrastructure.ExternalServices;

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
        if (request.Headers.Authorization == null)
        {
            var authorizationHeader = _httpContextAccessor
                .HttpContext?
                .Request.Headers.Authorization
                .ToString();

            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                request.Headers.Authorization =
                    AuthenticationHeaderValue.Parse(authorizationHeader);
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}