using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PjSip.App.Models
{
     public class AgentConfig
    {
        public class LLMConfig
        {
            [Required]
            public required string Model { get; init; }
            
            [Range(0, 2)]
            public float Temperature { get; set; } = 0.7f;
            
            [Range(1, 4096)]
            public int MaxTokens { get; set; } = 512;
            
            [Required, Url]
            public required string OllamaEndpoint { get; init; }
               [NotMapped]
            public Dictionary<string, string>? Parameters { get; set; }

            public LLMConfig() { }

            [SetsRequiredMembers]
            public LLMConfig(string model, string ollamaEndpoint)
            {
                Model = model;
                OllamaEndpoint = ollamaEndpoint;
            }
        }

        public class WhisperConfig
        {
            [Required, Url]
            public required string Endpoint { get; init; }
            
            [StringLength(5)]
            public string Language { get; set; } = "en";
            
            [Range(1, 60)]
            public int Timeout { get; set; } = 30;
            
            public bool EnableTranslation { get; set; }

            public WhisperConfig() { }

            [SetsRequiredMembers]
            public WhisperConfig(string endpoint)
            {
                Endpoint = endpoint;
            }
        }

        public class AuralisConfig
        {
            [Required, Url]
            public required string Endpoint { get; init; }
            
            [Required]
            public required string ApiKey { get; set; }
            
            [Range(1, 60)]
            public int Timeout { get; set; } = 30;
            
            public bool EnableAnalytics { get; set; }

            public AuralisConfig() { }

            [SetsRequiredMembers]
            public AuralisConfig(string endpoint, string apiKey)
            {
                Endpoint = endpoint;
                ApiKey = apiKey;
            }
        }

        public int Id { get; set; }
        
        [Required]
        public required string AgentId { get; set; }
        
        [Required]
        public required LLMConfig LLM { get; set; }
        
        [Required]
        public required WhisperConfig Whisper { get; set; }
        
        [Required]
        public required AuralisConfig Auralis { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        [Range(0, 100)]
        public int Priority { get; set; } = 50;
        
        public bool IsEnabled { get; set; } = true;

        public AgentConfig() { }

        [SetsRequiredMembers]
        public AgentConfig(
            string agentId,
            LLMConfig llm,
            WhisperConfig whisper,
            AuralisConfig auralis)
        {
            AgentId = agentId;
            LLM = llm;
            Whisper = whisper;
            Auralis = auralis;
        }
    }
}
