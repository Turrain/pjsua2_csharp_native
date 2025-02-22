using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using PjSip.App.Utils;
using PjSua2.Lx;
using PjSua2.Native.pjsua2;
public class MediaPortManager : IDisposable
    {
        private readonly ConcurrentDictionary<int, MediaPort> _mediaPorts;
        private readonly ILogger<MediaPortManager> _logger;
         private readonly ILoggerFactory _loggerFactory;
   
        private readonly Endpoint _endpoint;
        private static int _portIndex = 0;
        private bool _disposed;

        public MediaPortManager(ILogger<MediaPortManager> logger, ILoggerFactory loggerFactory)
    {
        _mediaPorts = new ConcurrentDictionary<int, MediaPort>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _endpoint = ThreadSafeEndpoint.Instance.InstanceEndpoint;
    }

        /// <summary>
        /// Creates or retrieves a MediaPort for a given call ID.
        /// </summary>
    public MediaPort GetOrCreateMediaPort(int callId)
        {
            return _mediaPorts.GetOrAdd(callId, id =>
            {
                _logger.LogDebug("Creating new MediaPort for call {CallId}", id);
                var mediaPort = CreateMediaPort(id);
                return mediaPort;
            });
        }

        private MediaPort CreateMediaPort(int callId)
        {
            var mediaPort = new MediaPort(_loggerFactory.CreateLogger<MediaPort>());
            var mediaFormatAudio = new MediaFormatAudio();
            mediaFormatAudio.init(
                 1,
                    8000,
                    1,
                    20000,
                    16
            );

           string portName = $"Call-{callId}-Port-{Interlocked.Increment(ref _portIndex)}";
            try
            {
                mediaPort.createPort(portName, mediaFormatAudio);
                _logger.LogInformation("Created MediaPort {PortName} for call {CallId}, PortId: {PortId}", 
                    portName, callId, mediaPort.getPortId());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create MediaPort for call {CallId}", callId);
                throw;
            }

            return mediaPort;
        }

     

     

        /// <summary>
        /// Removes and disposes a MediaPort for a given call ID.
        /// </summary>
      public void RemoveMediaPort(int callId)
        {
            if (_mediaPorts.TryRemove(callId, out var mediaPort))
            {
                try
                {
                    mediaPort.Dispose();
                    _logger.LogInformation("Disposed MediaPort for call {CallId}", callId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing MediaPort for call {CallId}", callId);
                }
            }
        }

       public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var callId in _mediaPorts.Keys)
            {
                RemoveMediaPort(callId);
            }
            _mediaPorts.Clear();
            _logger.LogDebug("MediaPortManager disposed");
        }
    }
    public class MediaPort : AudioMediaPort
    {
        public VoiceActivityDetector _vad = new();
        private Queue<byte[]> _audioQueue = new Queue<byte[]>();
        private const int FRAME_SAMPLES = 160;
        private const int FRAME_BYTES = 160 * 2;
        private byte[] _pcmBuffer = null;
        private int _pcmBufferIndex = 0;
        private readonly ILogger<MediaPort> _logger;

        public Action<MediaFrame> onFrameRequestedOverride;
        public Action<MediaFrame> onFrameReceivedOverride;

        public MediaPort(ILogger<MediaPort> logger)
        {
            _logger = logger;
        }

      public override void onFrameRequested(MediaFrame frame)
        {
           
                _logger?.LogDebug("onFrameRequested on Thread-{ThreadId}", Thread.CurrentThread.ManagedThreadId);
                frame.type = pjmedia_frame_type.PJMEDIA_FRAME_TYPE_AUDIO;
                int requiredBytes = FRAME_BYTES;
                byte[] tempBuffer = new byte[requiredBytes];
                int bytesCopied = 0;

                lock (_audioQueue)
                {
                    while (bytesCopied < requiredBytes)
                    {
                        if (_pcmBuffer == null || _pcmBufferIndex >= _pcmBuffer.Length)
                        {
                            if (_audioQueue.Count > 0)
                            {
                                _pcmBuffer = _audioQueue.Dequeue();
                                _pcmBufferIndex = 0;
                            }
                            else
                            {
                                break;
                            }
                        }

                        int remainingBytes = requiredBytes - bytesCopied;
                        int availableBytes = _pcmBuffer.Length - _pcmBufferIndex;
                        int bytesToCopy = Math.Min(remainingBytes, availableBytes);

                        Array.Copy(_pcmBuffer, _pcmBufferIndex, tempBuffer, bytesCopied, bytesToCopy);
                        bytesCopied += bytesToCopy;
                        _pcmBufferIndex += bytesToCopy;
                    }
                }

                if (bytesCopied < requiredBytes)
                {
                    Array.Clear(tempBuffer, bytesCopied, requiredBytes - bytesCopied);
                }

                ByteVector bv = [.. tempBuffer];
                frame.buf = bv;
                frame.size = (uint)requiredBytes;
            
        }

        public override void onFrameReceived(MediaFrame frame)
        {
            
                _logger?.LogDebug("onFrameReceived on Thread-{ThreadId}", Thread.CurrentThread.ManagedThreadId);
                if (frame == null || frame.buf == null || frame.size == 0)
                {
                    return;
                }
                _vad.ProcessFrame(frame);
          
        }

        public void AddToQueue(byte[] audioData)
        {
            if (audioData == null || audioData.Length == 0) return;
            lock (_audioQueue)
            {
                _audioQueue.Enqueue(audioData);
            }
        }

        public void ClearQueue()
        {
            lock (_audioQueue)
            {
                _audioQueue.Clear();
            }
            _pcmBuffer = null;
            _pcmBufferIndex = 0;
        }
    }


    public static class MediaPortTest
    {
        private static Timer _audioFeedTimer;
        private static byte[] _pcmData;
        private static int _feedPosition = 0;
        private static volatile bool _isRunning = false;
        private static readonly object _lock = new();

        public static void RunTest(MediaPort mediaPort, ILogger logger, int durationSeconds = 10)
        {
            if (_isRunning)
            {
                logger.LogWarning("Test is already running.");
                return;
            }

            _isRunning = true;
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.wav");

            try
            {
                if (!File.Exists(filePath))
                {
                    logger.LogError("WAV file not found at: {FilePath}", filePath);
                    _isRunning = false;
                    return;
                }

                byte[] wavData = File.ReadAllBytes(filePath);
                int headerSize = 44; // Standard WAV header size
                if (wavData.Length <= headerSize)
                {
                    logger.LogError("WAV file is too small or corrupted: {FilePath}", filePath);
                    _isRunning = false;
                    return;
                }

                _pcmData = new byte[wavData.Length - headerSize];
                Array.Copy(wavData, headerSize, _pcmData, 0, _pcmData.Length);
                logger.LogInformation("Loaded {Length} bytes of PCM data from {FilePath}", _pcmData.Length, filePath);

                // Feed data in chunks every 20ms to simulate real-time audio
                _feedPosition = 0;
                _audioFeedTimer = new Timer(
                    state => FeedData(mediaPort, logger),
                    null,
                    0, // Start immediately
                    20 // 20ms interval
                );

                // Stop the test after the specified duration
                Thread.Sleep(durationSeconds * 1000);
                StopTest(logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred while running MediaPort test");
                StopTest(logger);
            }
        }

        private static void FeedData(MediaPort mediaPort, ILogger logger)
        {
            const int chunkSize = 320; // Matches FRAME_BYTES (160 samples * 2 bytes/sample)

            ThreadSafeEndpoint.Instance.ExecuteSafely(() =>
            {
                lock (_lock) // Synchronize access to _pcmData and _feedPosition
                {
                    if (!_isRunning) return;

                    if (_feedPosition >= _pcmData.Length)
                    {
                        _feedPosition = 0; // Loop the audio
                        logger.LogDebug("Looping audio playback at position 0");
                    }

                    int bytesRemaining = _pcmData.Length - _feedPosition;
                    int bytesToCopy = Math.Min(chunkSize, bytesRemaining);
                    byte[] chunk = new byte[chunkSize]; // Always allocate full chunk size
                    Array.Copy(_pcmData, _feedPosition, chunk, 0, bytesToCopy);

                    // Pad with zeros if the chunk is incomplete
                    if (bytesToCopy < chunkSize)
                    {
                        Array.Clear(chunk, bytesToCopy, chunkSize - bytesToCopy);
                        logger.LogDebug("Padded chunk with {Padding} bytes of silence", chunkSize - bytesToCopy);
                    }

                    _feedPosition += bytesToCopy;
                    mediaPort.AddToQueue(chunk);
                    logger.LogDebug("Fed {Bytes} bytes to MediaPort at position {Position}", chunkSize, _feedPosition);
                }
            });
        }

        private static void StopTest(ILogger logger)
        {
            ThreadSafeEndpoint.Instance.ExecuteSafely(() =>
            {
                lock (_lock)
                {
                    if (!_isRunning) return;

                    _isRunning = false;
                    _audioFeedTimer?.Dispose();
                    _audioFeedTimer = null;
                    _pcmData = null;
                    _feedPosition = 0;
                    logger.LogInformation("MediaPort test stopped");
                }
            });
        }
    }