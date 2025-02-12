using System.ComponentModel.DataAnnotations;

namespace PjSip.App.Models
{
    public class AgentConfig
    {
        public int Id { get; set; }
        
        [Required]
        public string AgentId { get; set; }

        [Required]
        public Dictionary<string, string> LLMConfig { get; set; }

        [Required]
        [Url]
        public string AuralisEndpoint { get; set; }

        [Required]
        [Url]
        public string WhisperEndpoint { get; set; }

        [Required]
        [Url]
        public string OllamaEndpoint { get; set; }

        [Required]
        public string Model { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
