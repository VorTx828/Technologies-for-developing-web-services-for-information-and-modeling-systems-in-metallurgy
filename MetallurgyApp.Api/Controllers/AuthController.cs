using MetallurgyApp.Api.Data;
using MetallurgyApp.Api.Models;
using MetallurgyApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MetallurgyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokens;

    public AuthController(AppDbContext db, ITokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Register(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.UserName == dto.UserName))
            return Conflict("Пользователь уже существует");

        // Первый пользователь — админ автоматически
        var role = await _db.Users.AnyAsync() ? dto.Role : UserRole.Admin;

        var user = new User
        {
            UserName = dto.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(_tokens.CreateToken(user));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Неверный логин или пароль");

        return Ok(_tokens.CreateToken(user));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var id = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")!.Value);
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        return Ok(new { user.Id, user.UserName, Role = user.Role.ToString() });
    }
}