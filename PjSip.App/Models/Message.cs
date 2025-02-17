using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PjSip.App.Models
{
public class Message
{
    public int Id { get; set; }
    public required int ChatId { get; set; }
    public required string Sender { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

       public Message() { }

    [SetsRequiredMembers]
    public Message(int chatId, string sender, string content)
    {
        ChatId = chatId;
        Sender = sender;
        Content = content;
    }
}

}
