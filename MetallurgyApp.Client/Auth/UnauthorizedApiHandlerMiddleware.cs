using System.Net;

namespace MetallurgyApp.Client.Auth;

public class UnauthorizedApiHandlerMiddleware
{
    private readonly RequestDelegate _next;
    public UnauthorizedApiHandlerMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            ctx.Response.Cookies.Delete(AuthConstants.TokenCookieName);
            ctx.Response.Redirect("/Account/Login");
        }
    }
}