using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PjSua2.Lx.AudioStream
{
    public class WhisperClient : WebSocketClient
    {
        public WhisperClient(string uri) : base(uri) { }

        /// <summary>
        /// In Whisper, when sending JSON we encode the JSON string into bytes and send as binary.
        /// </summary>
        public override async Task SendJsonAsync<T>(T obj, CancellationToken cancellationToken = default)
        {
            string json = JsonSerializer.Serialize(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            await SendBinaryAsync(data, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// In Whisper, we expect to receive JSON messages sent as text. 
        /// Any non-text messages are ignored.
        /// </summary>
        protected override Task ProcessReceivedData(MemoryStream ms, WebSocketReceiveResult result)
        {
            if (result.MessageType != WebSocketMessageType.Text)
            {
                // Ignore non-text messages.
                return Task.CompletedTask;
            }

            ms.Seek(0, SeekOrigin.Begin);
            string message = Encoding.UTF8.GetString(ms.ToArray());
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(message))
                {
                    JsonElement clonedElement = doc.RootElement.Clone();
                    RaiseOnJsonMessage(clonedElement);
                }
            }
            catch (JsonException ex)
            {
                RaiseOnError(ex);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Optionally, provide a convenience method for sending audio (raw binary) data.
        /// </summary>
        public Task SendAudioAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            return SendBinaryAsync(data, cancellationToken);
        }
    }
}
