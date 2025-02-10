using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PjSua2.Lx.AudioStream
{
    public class AuralisClient : WebSocketClient
    {
        public AuralisClient(string uri) : base(uri) { }

        /// <summary>
        /// In Auralis, sending JSON is supported using the base implementation.
        /// We discourage sending binary data by hiding the base method.
        /// </summary>
        public new Task SendBinaryAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Auralis client does not support sending binary data. Use SendJsonAsync instead.");
        }

        /// <summary>
        /// In Auralis, we expect to receive binary messages.
        /// Text messages are ignored.
        /// </summary>
        protected override Task ProcessReceivedData(MemoryStream ms, WebSocketReceiveResult result)
        {
            if (result.MessageType != WebSocketMessageType.Binary)
            {
                // Ignore non-binary messages.
                return Task.CompletedTask;
            }

            byte[] data = ms.ToArray();
            RaiseOnBinaryMessage(data);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Optionally, provide a convenience method for sending commands as JSON.
        /// </summary>
        public Task SendCommandAsync<T>(T obj, CancellationToken cancellationToken = default)
        {
            return SendJsonAsync(obj, cancellationToken);
        }
    }
}
