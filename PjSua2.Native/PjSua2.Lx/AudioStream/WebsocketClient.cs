using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PjSua2.Lx.AudioStream{
  /// <summary>
    /// A basic WebSocket client that supports sending/receiving JSON (text) and binary data.
    /// It runs its receive loop in a background Task and raises events on message receipt.
    /// </summary>
    public class WebSocketClient
    {
        private readonly Uri _uri;
        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cts;
        private Task _receiveTask;

        // Events (callbacks) for handling messages, errors, and connection closure.
        public event Action<string> OnTextMessage;
        public event Action<byte[]> OnBinaryMessage;
        public event Action<Exception> OnError;
        public event Action<WebSocketCloseStatus?, string> OnClosed;

        /// <summary>
        /// Initialize the client with the target WebSocket URI.
        /// </summary>
        /// <param name="uri">The URI of the WebSocket server (e.g. ws://example.com/socket)</param>
        public WebSocketClient(string uri)
        {
            _uri = new Uri(uri);
            _webSocket = new ClientWebSocket();
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Connect to the WebSocket server and start the receive loop.
        /// </summary>
        public async Task ConnectAsync()
        {
            await _webSocket.ConnectAsync(_uri, CancellationToken.None);
            // Start the receive loop in a background Task.
            _receiveTask = Task.Run(ReceiveLoop);
        }

        /// <summary>
        /// Send a text message (for example, JSON) to the server.
        /// </summary>
        public async Task SendTextAsync(string message)
        {
            if (_webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("Connection is not open.");

            byte[] encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(encoded);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
        }

        /// <summary>
        /// Send binary data to the server.
        /// </summary>
        public async Task SendBinaryAsync(byte[] data)
        {
            if (_webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("Connection is not open.");

            var buffer = new ArraySegment<byte>(data);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Binary, endOfMessage: true, cancellationToken: CancellationToken.None);
        }

        /// <summary>
        /// Runs in a background task. Receives incoming messages and dispatches them via events.
        /// </summary>
        private async Task ReceiveLoop()
        {
            // A buffer to store received data.
            var buffer = new byte[8192];
            try
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    // Receive a message.
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        // Close message received – notify subscribers and close the socket.
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", CancellationToken.None);
                        OnClosed?.Invoke(_webSocket.CloseStatus, _webSocket.CloseStatusDescription);
                        break;
                    }
                    else
                    {
                        // In case of fragmented messages, accumulate the data.
                        int count = result.Count;
                        while (!result.EndOfMessage)
                        {
                            if (count >= buffer.Length)
                            {
                                OnError?.Invoke(new Exception("Message too long to handle."));
                                break;
                            }
                            result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, count, buffer.Length - count), _cts.Token);
                            count += result.Count;
                        }

                        // Dispatch based on the message type.
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var msg = Encoding.UTF8.GetString(buffer, 0, count);
                            OnTextMessage?.Invoke(msg);
                        }
                        else if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            var data = new byte[count];
                            Array.Copy(buffer, data, count);
                            OnBinaryMessage?.Invoke(data);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the cancellation token is triggered.
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
        }

        /// <summary>
        /// Disconnect from the WebSocket server.
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                _cts.Cancel();
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
            }
        }
    }

}