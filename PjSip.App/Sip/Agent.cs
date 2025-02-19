using PjSip.App.Models;
using PjSua2.Lx.AudioStream;
using PjSua2.Lx.GenStream;

namespace PjSip.App.Sip {
     public class Agent : IAsyncDisposable
    {
        private readonly AgentConfig _configuration;
        public AuralisClient AuralisClient { get; private set; }
        public WhisperClient WhisperClient { get; private set; }
        public OllamaStreamingService OllamaStreamingService { get; private set; }
        public List<string> History { get; private set; }

        public Agent(AgentConfig configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            History = new List<string>();

            AuralisClient = new AuralisClient(_configuration.Auralis.Endpoint);
            WhisperClient = new WhisperClient(_configuration.Whisper.Endpoint);
            OllamaStreamingService = new OllamaStreamingService();

            // Subscribe to events from Auralis.
            AuralisClient.OnBinaryMessage += data =>
            {
                // For example, log the binary data or update internal state.
            };
            AuralisClient.OnError += ex =>
            {
                Console.WriteLine("Auralis Error: " + ex.Message);
             
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
                temperature = _configuration.LLM.Temperature
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