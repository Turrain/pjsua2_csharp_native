using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PjSua2.Lx.AudioStream;
using PjSua2.Lx.Configuration;
using PjSua2.Lx.GenStream;
using PjSua2.Native;

namespace PjSua2.Lx
{
    /// <summary>
    /// An interface for managing agent configurations and instances.
    /// </summary>
    public interface IAgentManager
    {
        /// <summary>
        /// Adds or updates an agent configuration with the specified identifier.
        /// </summary>
        void AddConfiguration(string configId, AgentConfiguration configuration);

        /// <summary>
        /// Gets (or creates) an agent instance for the given agentId.
        /// </summary>
        Agent GetAgent(string agentId);

        /// <summary>
        /// Removes and disposes an agent by its identifier.
        /// </summary>
        Task RemoveAgentAsync(string agentId);

        /// <summary>
        /// Retrieves all agent configurations.
        /// </summary>
        IDictionary<string, AgentConfiguration> GetConfigurations();
    }

     /// <summary>
    /// A concrete implementation of IAgentManager.
    /// </summary>
    public class AgentManager : IAgentManager
    {
        private readonly Dictionary<string, AgentConfiguration> _configurations;
        private readonly Dictionary<string, Agent> _agents;

        public AgentManager()
        {
            _configurations = new Dictionary<string, AgentConfiguration>();
            _agents = new Dictionary<string, Agent>();

            // Add a default configuration.
            var defaultConfig = new AgentConfiguration
            {
                AgentId = "default",
                Auralis = new WebSocketConfig 
                { 
                    Endpoint = "ws://37.151.89.206:8766", 
                    AutoReconnect = true, 
                    ReconnectInterval = 5000 
                },
                Whisper = new WebSocketConfig 
                { 
                    Endpoint = "ws://37.151.89.206:8765", 
                    AutoReconnect = true, 
                    ReconnectInterval = 5000 
                },
                Ollama = new OllamaConfig
                {
                    Endpoint = "https://models.aitomaton.online/api/generate",
                    Model = "phi4",
                    Temperature = 0.9F,
                    MaxBufferLength = 100
                }
            };

            AddConfiguration("default", defaultConfig);
        }

        public void AddConfiguration(string configId, AgentConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configId))
                throw new ArgumentException("Configuration ID cannot be null or empty", nameof(configId));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _configurations[configId] = configuration;
        }

        public Agent GetAgent(string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
                agentId = "default";

            if (_agents.TryGetValue(agentId, out var existingAgent))
                return existingAgent;

            // Use the specific configuration if it exists; otherwise, use default.
            var config = _configurations.ContainsKey(agentId) ? _configurations[agentId] : _configurations["default"];
            var newAgent = new Agent(config);
            _agents[agentId] = newAgent;
            return newAgent;
        }

        public async Task RemoveAgentAsync(string agentId)
        {
            if (_agents.TryGetValue(agentId, out var agent))
            {
                await agent.DisposeAsync();
                _agents.Remove(agentId);
            }
        }

        public IDictionary<string, AgentConfiguration> GetConfigurations() => _configurations;
    }

    /// <summary>
    /// Represents an agent that establishes connections to external services and processes messages.
    /// </summary>
    public class Agent : IAsyncDisposable
    {
        private readonly AgentConfiguration _configuration;
        public AuralisClient AuralisClient { get; private set; }
        public WhisperClient WhisperClient { get; private set; }
        public OllamaStreamingService OllamaStreamingService { get; private set; }
        /// <summary>
        /// A simple history log of string messages. In a production scenario, you might store a more complex object.
        /// </summary>
        public List<string> History { get; private set; }

        public Agent(AgentConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            History = new List<string>();

            AuralisClient = new AuralisClient(_configuration.Auralis.Endpoint);
            WhisperClient = new WhisperClient(_configuration.Whisper.Endpoint);
            OllamaStreamingService = new OllamaStreamingService(_configuration.Ollama.MaxBufferLength);

            // Subscribe to events from Auralis.
            AuralisClient.OnBinaryMessage += data =>
            {
                // For example, log the binary data or update internal state.
            };
            AuralisClient.OnError += ex =>
            {
                Console.WriteLine("Auralis Error: " + ex.Message);
                if (_configuration.Auralis.AutoReconnect)
                {
                    Task.Delay(_configuration.Auralis.ReconnectInterval)
                        .ContinueWith(_ => AuralisClient.ConnectAsync());
                }
            };

            // Subscribe to events from Whisper.
            WhisperClient.OnJsonMessage += json =>
            {
                Console.WriteLine("Whisper Received JSON: " + json.ToString());
                if (json.TryGetProperty("text", out var textElement))
                {
                    var text = textElement.GetString();
                    if (!string.IsNullOrEmpty(text))
                    {
                        // Process the recognized text.
                        Think(text);
                    }
                }
            };
            WhisperClient.OnError += ex =>
            {
                Console.WriteLine("Whisper Error: " + ex.Message);
                if (_configuration.Whisper.AutoReconnect)
                {
                    Task.Delay(_configuration.Whisper.ReconnectInterval)
                        .ContinueWith(_ => WhisperClient.ConnectAsync());
                }
            };

            // When a sentence is ready from the Ollama stream, speak it via Auralis.
            OllamaStreamingService.SentenceReady += sentence =>
            {
                _ = Speak(sentence);
            };

            // Initialize connections asynchronously.
            InitializeConnections();
        }

        /// <summary>
        /// Connects to the external services.
        /// </summary>
        private async void InitializeConnections()
        {
            try
            {
                await AuralisClient.ConnectAsync();
                await WhisperClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing connections: {ex.Message}");
            }
        }

        /// <summary>
        /// Forwards audio data to the Whisper service.
        /// </summary>
        public void Listen(byte[] framesData)
        {
            WhisperClient.SendAudioAsync(framesData);
        }

        /// <summary>
        /// Processes input text by starting a streaming response via Ollama.
        /// </summary>
        public void Think(string input)
        {
            OllamaStreamingService.StartStreamingAsync(input);
        }

        /// <summary>
        /// Sends the given text to the Auralis service for “speaking.”
        /// </summary>
        public async Task Speak(string input)
        {
            var command = new
            {
                input = input,
                voice = "default",
                stream = true,
                temperature = _configuration.Ollama.Temperature
            };

            await AuralisClient.SendCommandAsync(command);
            // Optionally, update the history.
            History.Add("Agent: " + input);
        }

        public async ValueTask DisposeAsync()
        {
            if (AuralisClient != null)
                await AuralisClient.DisposeAsync();
            if (WhisperClient != null)
                await WhisperClient.DisposeAsync();
        }
    }
}
