using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MetallurgyApp.Client.Auth;

/// <summary>Читает JWT из cookies, валидирует (сервер уже проверил его — 
/// здесь доверяем, т.к. cookie HttpOnly и пришла с нашего же сервера), 
/// и восстанавливает ClaimsPrincipal для MVC.</summary>
public static class JwtCookieEvents
{
    public static async Task OnValidatePrincipal(CookieValidatePrincipalContext ctx)
    {
        var token = ctx.Request.Cookies[AuthConstants.TokenCookieName];
        if (string.IsNullOrEmpty(token))
        {
            ctx.RejectPrincipal();
            await ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt;
        try { jwt = handler.ReadJwtToken(token); }
        catch
        {
            ctx.RejectPrincipal();
            await ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return;
        }

        if (jwt.ValidTo < DateTime.UtcNow)
        {
            ctx.RejectPrincipal();
            await ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}