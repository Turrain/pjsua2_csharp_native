using System.Collections.Generic;

namespace ChatApi.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string APIKey { get; set; }
        public string EndpointUrl { get; set; }
        public int Timeout { get; set; } // in milliseconds
        public string AdditionalConfig { get; set; } // Could be a JSON string with extra settings

        // Navigation property: an agent can have multiple chats
        public List<Chat> Chats { get; set; }
    }
}
