using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PjSua2.Lx.AudioStream
{
    /// <summary>
    /// A basic WebSocket client that supports sending/receiving JSON (text) and binary data.
    /// It runs its receive loop in a background task and raises events on message receipt.
    /// </summary>
    public class WebSocketClient : IAsyncDisposable
    {
        private readonly Uri _uri;
        private ClientWebSocket _webSocket;
        private readonly CancellationTokenSource _cts;
        private Task _receiveTask;

        // Events for handling messages, errors, and connection closure.
        public event Action<string> OnTextMessage;
        public event Action<JsonElement> OnJsonMessage;
        public event Action<byte[]> OnBinaryMessage;
        public event Action<Exception> OnError;
        public event Action<WebSocketCloseStatus?, string> OnClosed;

        /// <summary>
        /// Initialize the client with the target WebSocket URI.
        /// </summary>
        public WebSocketClient(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("URI cannot be null or empty.", nameof(uri));

            _uri = new Uri(uri);
            _webSocket = new ClientWebSocket();
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Connect to the WebSocket server and start the receive loop.
        /// </summary>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            await _webSocket.ConnectAsync(_uri, cancellationToken).ConfigureAwait(false);
            _receiveTask = Task.Run(ReceiveLoop, cancellationToken);
        }

        /// <summary>
        /// Send a text message (for example, JSON) to the server.
        /// </summary>
        public async Task SendTextAsync(string message, CancellationToken cancellationToken = default)
        {
            if (_webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("Connection is not open.");

            byte[] encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(encoded);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
        }

        /// <summary>
        /// Send binary data to the server.
        /// </summary>
        public async Task SendBinaryAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            if (_webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("Connection is not open.");

            var buffer = new ArraySegment<byte>(data);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Binary, endOfMessage: true, cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
        }

        /// <summary>
        /// Send a JSON message to the server. The object is serialized to JSON and sent as a text message.
        /// (Derived classes may override this behavior.)
        /// </summary>
        public virtual async Task SendJsonAsync<T>(T obj, CancellationToken cancellationToken = default)
        {
            string json = JsonSerializer.Serialize(obj);
            await SendTextAsync(json, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs in a background task. Receives incoming messages and dispatches them via events.
        /// </summary>
       private async Task ReceiveLoop()
    {
        try
        {
            var buffer = new byte[8192];
            while (_webSocket.State == WebSocketState.Open)
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;
                
                try
                {
                    do
                    {
                        result = await _webSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer), 
                            _cts.Token
                        ).ConfigureAwait(false);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await HandleCloseMessage().ConfigureAwait(false);
                            return;
                        }

                        await ms.WriteAsync(buffer.AsMemory(0, result.Count), _cts.Token)
                            .ConfigureAwait(false);
                    }
                    while (!result.EndOfMessage);

                    await ProcessReceivedData(ms, result).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (_cts.Token.IsCancellationRequested)
                {
                    // Clean exit when cancellation is requested
                    return;
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            RaiseOnError(ex);
        }
    }

    private async Task HandleCloseMessage()
    {
        try
        {
            await _webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closed by the server",
                CancellationToken.None  // Use None to ensure clean shutdown
            ).ConfigureAwait(false);
        }
        finally
        {
            OnClosed?.Invoke(_webSocket.CloseStatus, _webSocket.CloseStatusDescription);
        }
    }



        /// <summary>
        /// Processes the received message. By default, if the message is text it is raised via OnTextMessage
        /// and then an attempt is made to parse it as JSON (raising OnJsonMessage); if binary it is raised via OnBinaryMessage.
        /// Derived classes may override this to change behavior.
        /// </summary>
        protected virtual Task ProcessReceivedData(MemoryStream ms, WebSocketReceiveResult result)
        {
            ms.Seek(0, SeekOrigin.Begin);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(ms.ToArray());
                RaiseOnTextMessage(message);
                try
                {
                    using (JsonDocument doc = JsonDocument.Parse(message))
                    {
                        
                        JsonElement clonedElement = doc.RootElement.Clone();
                        RaiseOnJsonMessage(clonedElement);
                    }
                }
                catch (JsonException)
                {
                    // The text message was not valid JSON.
                }
            }
            else if (result.MessageType == WebSocketMessageType.Binary)
            {
                byte[] data = ms.ToArray();
                RaiseOnBinaryMessage(data);
            }
            return Task.CompletedTask;
        }

        // Protected helper methods to raise events. Derived classes should use these instead of invoking the events directly.
        protected void RaiseOnTextMessage(string message) => OnTextMessage?.Invoke(message);
        protected void RaiseOnJsonMessage(JsonElement element) => OnJsonMessage?.Invoke(element);
        protected void RaiseOnBinaryMessage(byte[] data) => OnBinaryMessage?.Invoke(data);
        protected void RaiseOnError(Exception ex) => OnError?.Invoke(ex);

        /// <summary>
        /// Disconnect from the WebSocket server.
        /// </summary>
         public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_webSocket.State != WebSocketState.Open && 
            _webSocket.State != WebSocketState.CloseReceived)
            return;

        try
        {
            _cts.Cancel();
            await _webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Client disconnecting",
                cancellationToken
            ).ConfigureAwait(false);

            if (_receiveTask != null)
                await _receiveTask.ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            RaiseOnError(ex);
        }
    }

        /// <summary>
        /// Disposes the WebSocket client and releases all associated resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_webSocket != null)
                {
                    if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.CloseReceived)
                    {
                        await DisconnectAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseOnError(ex);
            }
            finally
            {
                _webSocket?.Dispose();
                _cts.Dispose();
            }
        }
    }
}
