using System;

namespace ChatApi.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string Sender { get; set; } // e.g., "user" or "agent"
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
