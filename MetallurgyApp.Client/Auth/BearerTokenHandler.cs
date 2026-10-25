using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace MetallurgyApp.Client.Auth;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _http;

    public BearerTokenHandler(IHttpContextAccessor http) => _http = http;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        if (request.Headers.Authorization is null)
        {
            var token = _http.HttpContext?.Request.Cookies[AuthConstants.TokenCookieName];
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, ct);
    }
}