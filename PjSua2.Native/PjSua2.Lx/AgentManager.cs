using System;
using PjSua2.Lx.AudioStream;
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
        public List<string> history { get; set; } = new();



        public Agent()
        {
            auralisClient.OnBinaryMessage += data =>
            {
                Console.WriteLine("Auralis Received Binary: " + BitConverter.ToString(data));
            };
            auralisClient.OnError += ex =>
            {
                Console.WriteLine("Auralis Error: " + ex.Message);
            };

            whisperClient.OnJsonMessage += json =>
            {
                Console.WriteLine("Whisper Received JSON: " + json.ToString());
                Console.WriteLine(json.GetProperty("text"));
            };
            whisperClient.OnError += ex =>
            {
                Console.WriteLine("Whisper Error: " + ex.Message);
            };
        }

        public void Listen(ReadOnlyMemory<Native.pjsua2.MediaFrame> voiceFrames) {
            
         }
        public void Think(string input) { }
        public void Speak(string input) { }
    }
}