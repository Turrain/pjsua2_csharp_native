using System;

namespace PjSua2.Lx.Configuration
{
    public class AgentConfiguration
    {
        public string AgentId { get; set; }
        public WebSocketConfig Auralis { get; set; }
        public WebSocketConfig Whisper { get; set; }
        public OllamaConfig Ollama { get; set; }

        public AgentConfiguration()
        {
            Auralis = new WebSocketConfig();
            Whisper = new WebSocketConfig();
            Ollama = new OllamaConfig();
        }
    }

    public class WebSocketConfig
    {
        public string Endpoint { get; set; }
        public int ReconnectInterval { get; set; } = 5000; // Default 5 seconds
        public bool AutoReconnect { get; set; } = true;
    }

    public class OllamaConfig
    {
        public string Endpoint { get; set; }
        public string Model { get; set; }
        public float Temperature { get; set; }
        public int MaxBufferLength { get; set; } = 100;
    }
}
