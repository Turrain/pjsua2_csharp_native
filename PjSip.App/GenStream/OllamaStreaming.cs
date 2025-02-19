using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PjSua2.Lx.GenStream
{
    // Represents a single token from the streaming response.
    public class TokenResponse
    {
        public string model { get; set; }
        public DateTime created_At { get; set; }
        public string response { get; set; }
        public bool done { get; set; }
    }

    /// <summary>
    /// This service sends a POST request to an Ollama-style streaming endpoint,
    /// accumulates response tokens into sentences, and raises an event when a sentence is ready.
    /// </summary>
    public class OllamaStreamingService
    {
        private readonly HttpClient _httpClient;

        private readonly Queue<string> _sentenceQueue;
        private readonly StringBuilder _buffer;
        private readonly int _maxBufferLength; // flush buffer if sentence grows too long

        /// <summary>
        /// Occurs when a complete (or nearly complete) sentence is ready to be voiced out.
        /// </summary>
        public event Action<string> SentenceReady;

        /// <summary>
        /// Initializes a new instance of the <see cref="OllamaStreamingService"/> class.
        /// </summary>
        /// <param name="streamingEndpoint">The full URL of the streaming API endpoint (e.g., "http://localhost:11434/api/generate").</param>
        /// <param name="maxBufferLength">If no punctuation is encountered, flush the sentence once this length is exceeded.</param>
        public OllamaStreamingService(int maxBufferLength = 100)
        {
            _httpClient = new HttpClient();
     
            _sentenceQueue = new Queue<string>();
            _buffer = new StringBuilder();
            _maxBufferLength = maxBufferLength;
        }

        /// <summary>
        /// Begins streaming the response from the API.
        /// </summary>
        public async Task StartStreamingAsync(string input)
        {
            // Build the request payload. Adjust as needed.
            var requestData = new
            {
                model = "phi4",         // the model name
                prompt = input,
                stream = true           // enable streaming
            };

            string jsonRequest = JsonSerializer.Serialize(requestData);
            using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Send POST request with ResponseHeadersRead so we can start reading the stream as it arrives.
            using var response = await _httpClient.PostAsync("https://models.aitomaton.online/api/generate", content);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            
            while (!reader.EndOfStream)
            {

                string line = await reader.ReadLineAsync();
               
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    // Deserialize each JSON token from the stream.
                    var token = JsonSerializer.Deserialize<TokenResponse>(line);
                    if (token != null)
                    {
                        ProcessToken(token);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deserializing token: " + ex.Message);
                }
            }

            // At the end of the stream, flush any remaining content.
            FlushBuffer();
        }

        /// <summary>
        /// Processes a token response by appending its text to the buffer and checking for sentence completion.
        /// </summary>
        /// <param name="token">The token response from the API.</param>
        private void ProcessToken(TokenResponse token)
        {
            // Append the token text. (Assumes tokens may include leading spaces as needed.)
            _buffer.Append(token.response);
           
            // Optionally, if the token indicates the stream is done, flush the buffer.
            if (token.done)
            {
                FlushBuffer();
                return;
            }

            // If this token ends with a sentence-ending punctuation, flush the sentence.
            if (IsSentenceComplete(token.response))
            {
                FlushBuffer();
                return;
            }

            // Also, if the buffer grows too long (i.e. no punctuation encountered), flush it as an "almost complete" sentence.
            if (_buffer.Length >= _maxBufferLength)
            {
                FlushBuffer();
            }
        }

        /// <summary>
        /// Determines whether the token text indicates the end of a sentence.
        /// </summary>
        /// <param name="tokenText">The text of the token.</param>
        /// <returns><c>true</c> if the token ends with a sentence terminator; otherwise, <c>false</c>.</returns>
        private bool IsSentenceComplete(string tokenText)
        {
            if (string.IsNullOrEmpty(tokenText))
            {
                return false;
            }

            // Check if the last character is a common sentence terminator.
            char lastChar = tokenText[tokenText.Length - 1];
            return lastChar == '.' || lastChar == '!' || lastChar == '?' ;
        }

        /// <summary>
        /// Flushes the current buffer into a sentence, enqueues it, and fires the delegate.
        /// </summary>
        private void FlushBuffer()
        {
            if (_buffer.Length > 0)
            {
                string sentence = _buffer.ToString().Trim();
                if (!string.IsNullOrEmpty(sentence))
                {
                    _sentenceQueue.Enqueue(sentence);
                    // Fire the event so that the sentence can be "voiced out" (or processed).
                    SentenceReady?.Invoke(sentence);
                }
                _buffer.Clear();
            }
        }

        /// <summary>
        /// Gets the queue of complete sentences.
        /// </summary>
        public Queue<string> SentenceQueue => _sentenceQueue;
    }

  
}
