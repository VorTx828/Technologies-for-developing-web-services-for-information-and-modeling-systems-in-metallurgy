namespace MetallurgyApp.Api.Models;

public class ChatMessage
{
    public Guid Id { get; set; } = Guid.CreateVersion7();   // .NET 9+
    public string UserName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}