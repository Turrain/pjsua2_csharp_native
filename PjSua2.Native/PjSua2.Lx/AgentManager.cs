using System;
using System.Threading.Tasks;
using PjSua2.Lx.AudioStream;
using PjSua2.Lx.GenStream;
using PjSua2.Native;
namespace PjSua2.Lx
{


    public class AgentManager
    {
        private static readonly Lazy<AgentManager> _instance =
            new Lazy<AgentManager>(() => new AgentManager());

        public static AgentManager Instance => _instance.Value;

        public Agent GetAgent(string agentId)
        {
            // Implementation
            return new Agent();
        }
    }

    public class Agent
    {
        public AuralisClient auralisClient { get; set; } = new AuralisClient("ws://37.151.89.206:8766");
        public WhisperClient whisperClient { get; set; } = new WhisperClient("ws://37.151.89.206:8765");
        public OllamaStreamingService ollamaStreamingService { get; set; } = new();
        public List<string> history { get; set; } = new();



        public Agent()
        {
            auralisClient.OnBinaryMessage += data =>
            {
           //     Console.WriteLine("Auralis Received Binary: " + BitConverter.ToString(data));
            };
            auralisClient.OnError += ex =>
            {
                Console.WriteLine("Auralis Error: " + ex.Message);
            };

            whisperClient.OnJsonMessage += json =>
            {
                Console.WriteLine("Whisper Received JSON: " + json.ToString());
                Console.WriteLine(json.GetProperty("text"));
                Think(json.GetProperty("text").GetString());
            };
            whisperClient.OnError += ex =>
            {
                Console.WriteLine("Whisper Error: " + ex.Message);
            };
            ollamaStreamingService.SentenceReady += sentenceReady => {
                Speak(sentenceReady);
            };

            auralisClient.ConnectAsync();
            whisperClient.ConnectAsync();
        }

        public void Listen(byte[] framesData)
        {
            whisperClient.SendAudioAsync(framesData);
        }
        public void Think(string input) { 
            ollamaStreamingService.StartStreamingAsync(input);
        }
        public async Task Speak(string input)
        {
            var json = new
            {
                input = input,
                voice = "default",
                stream = true,
                temperature = 0.9
            };
          
            await auralisClient.SendCommandAsync(json);
        }
    }
}

//     var input = new
//     {
//         input = "Hello, world!",
//         voice = "default",
//         stream = true,
//         temperature = 0.5
//     };