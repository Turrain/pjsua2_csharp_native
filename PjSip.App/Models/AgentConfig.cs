using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PjSip.App.Models
{
 public class AgentConfig
{
    public int Id { get; set; }
    public required string AgentId { get; set; }
    public required Dictionary<string, string> LLMConfig { get; set; }
    public required string AuralisEndpoint { get; set; }
    public required string WhisperEndpoint { get; set; }
    public required string OllamaEndpoint { get; set; }
    public required string Model { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsEnabled { get; set; } = true;

      public AgentConfig() { }

    [SetsRequiredMembers]
    public AgentConfig(
        string agentId, 
        Dictionary<string, string> llmConfig,
        string auralisEndpoint,
        string whisperEndpoint,
        string ollamaEndpoint,
        string model)
    {
        AgentId = agentId;
        LLMConfig = llmConfig;
        AuralisEndpoint = auralisEndpoint;
        WhisperEndpoint = whisperEndpoint;
        OllamaEndpoint = ollamaEndpoint;
        Model = model;
    }
}
}
