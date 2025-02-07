using System.Net.WebSockets;

namespace PjSua2.Lx.AudioStream
{
    class WhisperWebsocketClient : WebSocketClient
    {
        public WhisperWebsocketClient(string uri) : base(uri)
        {
            this.OnTextMessage += HandleTextMessage;
            this.OnBinaryMessage += HandleBinaryMessage;
            this.OnError += HandleError;
            this.OnClosed += HandleClosed;
        }
        private void HandleTextMessage(string msg)
        {
            // Custom logic for JSON messages.
            Console.WriteLine("Custom handler received text: " + msg);
            // You could deserialize JSON here (using e.g. Newtonsoft.Json or System.Text.Json).
        }

        private void HandleBinaryMessage(byte[] data)
        {
            // Custom logic for binary messages.
            Console.WriteLine("Custom handler received binary data, length: " + data.Length);
        }

        private void HandleError(Exception ex)
        {
            Console.WriteLine("Custom handler error: " + ex.Message);
        }

        private void HandleClosed(WebSocketCloseStatus? status, string description)
        {
            Console.WriteLine($"Connection closed: {status}, {description}");
        }
    }
}