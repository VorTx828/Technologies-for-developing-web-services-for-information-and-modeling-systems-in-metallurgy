using MetallurgyApp.Api.Data;
using MetallurgyApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MetallurgyApp.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly AppDbContext _db;
    public ChatHub(AppDbContext db) => _db = db;

    public async Task SendMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        var userName = Context.User?.Identity?.Name ?? "unknown";
        var msg = new ChatMessage
        {
            UserName = userName,
            Text = text.Trim(),
            SentAt = DateTime.UtcNow
        };
        _db.ChatMessages.Add(msg);
        await _db.SaveChangesAsync();

        await Clients.All.SendAsync("ReceiveMessage",
            msg.Id, msg.UserName, msg.Text, msg.SentAt);
    }

    public async Task<IEnumerable<ChatMessage>> GetHistory(int count = 50)
    {
        if (count <= 0 || count > 200) count = 50;
        return await _db.ChatMessages
            .OrderByDescending(m => m.SentAt)
            .Take(count)
            .OrderBy(m => m.SentAt)
            .AsNoTracking()
            .ToListAsync();
    }
}