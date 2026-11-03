using MetallurgyApp.Client.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetallurgyApp.Client.Controllers;

[Authorize]
public class ChatController : Controller
{
    public IActionResult Index()
    {
        var token = Request.Cookies[AuthConstants.TokenCookieName];
        ViewBag.AccessToken = token ?? string.Empty;
        ViewBag.HubUrl = "https://localhost:7001/hubs/chat";
        return View();
    }
}