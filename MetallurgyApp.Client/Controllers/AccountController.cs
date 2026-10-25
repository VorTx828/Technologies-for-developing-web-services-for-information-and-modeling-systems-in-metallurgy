using System.Security.Claims;
using MetallurgyApp.Client.Auth;
using MetallurgyApp.Client.Models;
using MetallurgyApp.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetallurgyApp.Client.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _api;
    public AccountController(ApiClient api) => _api = api;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginDto());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(dto);

        var token = await _api.LoginAsync(dto);
        if (token is null)
        {
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return View(dto);
        }

        await SignInWithTokenAsync(token);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "ProductionAreas");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterDto());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var (ok, error) = await _api.RegisterAsync(dto);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Ошибка регистрации");
            return View(dto);
        }

        var token = await _api.LoginAsync(new LoginDto
        {
            UserName = dto.UserName,
            Password = dto.Password
        });
        if (token is not null) await SignInWithTokenAsync(token);

        return RedirectToAction("Index", "ProductionAreas");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete(AuthConstants.TokenCookieName);
        return RedirectToAction("Login");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();


    private async Task SignInWithTokenAsync(TokenResponseDto token)
    {
        Response.Cookies.Append(
            AuthConstants.TokenCookieName,
            token.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = token.ExpiresAt.ToUniversalTime(),
                Path = "/"
            });

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, token.UserName),
            new(ClaimTypes.Role, token.Role.ToString())
        };

        var identity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = token.ExpiresAt.ToUniversalTime()
            });
    }
}