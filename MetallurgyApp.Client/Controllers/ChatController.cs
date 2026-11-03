using MetallurgyApp.Client.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetallurgyApp.Client.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly IConfiguration _config;
    public ChatController(IConfiguration config) => _config = config;

    public IActionResult Index()
    {
        var token = Request.Cookies[AuthConstants.TokenCookieName];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        ViewBag.AccessToken = token;
        ViewBag.HubUrl = $"{(_config["ApiBaseUrl"]?.TrimEnd('/') ?? "http://localhost:7100")}/hubs/chat";
        ViewBag.IsAdmin = User.IsInRole("Admin");
        return View();
    }
}