using System;
using System.Collections.Generic;

namespace ChatApi.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property: a chat can have multiple messages
        public List<Message> Messages { get; set; }
    }
}
