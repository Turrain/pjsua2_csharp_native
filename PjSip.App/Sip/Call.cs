using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PjSua2.Native.pjsua2;
using PjSip.App.Services;
using PjSip.App.Exceptions;
using PjSip.App.Data;
using PjSip.App.Utils;
using Microsoft.AspNetCore.SignalR;
using PjSip.App.Hubs;
using Microsoft.EntityFrameworkCore;

namespace PjSip.App.Sip
{
    public class Call : PjSua2.Native.pjsua2.Call, IDisposable
    {
         private static bool _hasSpoken = false;
        private readonly IHubContext<SipHub> _hubContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<Call> _logger;
        private readonly MediaPortManager _mediaPortManager;
        private readonly Agent _agent;
        private MediaPort _mediaPort;

        public enum Direction { Incoming, Outgoing }
        public Direction CallDirection { get; set; }

        public Call(Account acc, int callId, ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory, MediaPortManager mediaPortManager, IHubContext<SipHub> hubContext)
              : base(acc, callId)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = loggerFactory.CreateLogger<Call>();
            _mediaPortManager = mediaPortManager ?? throw new ArgumentNullException(nameof(mediaPortManager));
            _agent = new Agent(acc.Agent);
            _hubContext = hubContext;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();
                try
                {
                    _logger.LogInformation("Call {CallId} initialized successfully", callId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize call {CallId}", callId);
                    throw new SipCallException("Failed to initialize call", callId, "INIT_FAILED", ex);
                }
            }

            _mediaPort = _mediaPortManager.GetOrCreateMediaPort(callId);
            SetupMediaPortCallbacks();
        }
        private async Task NotifyCallStateChange(string status)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

            var call = await context.SipCalls
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CallId == getId());

            if (call != null)
            {
                call.Status = status;
                await context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("CallUpdate", call);
            }
        }
        private void SetupMediaPortCallbacks()
        {
            _mediaPort._vad.VoiceSegmentDetected += (voiceFrames) =>
            {
                _logger.LogDebug("Voice segment detected for call {CallId}, length: {Length}", (int)base.getId(), voiceFrames.Length);
                var data = _mediaPort._vad.ExtractBytesFromFrames(voiceFrames.Span);
                _agent.Listen(data);
                _mediaPort.ClearQueue();
            };

            _agent.AuralisClient.OnBinaryMessage += (data) =>
            {
                Console.WriteLine($"Received binary message from agent {data.Length} bytes");
                _mediaPort.AddToQueue(data);
            };
        }
        public override void onCallState(OnCallStateParam prm)
        {
            try
            {
                var info = base.getInfo();
                _logger.LogInformation("Call {CallId} state changed to {State}", (int)base.getId(), info.stateText);
                _ = NotifyCallStateChange(info.stateText);
                if (info.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                {
                    _logger.LogInformation("Call {CallId} disconnected", (int)base.getId());
                    CleanupResources();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling call state change for {CallId}", (int)base.getId());
            }
        }
        public override void onCallMediaState(OnCallMediaStateParam prm)
        {
            _logger.LogInformation("Call {CallId} media state changed on Thread-{ThreadId}",
                (int)base.getId(), Thread.CurrentThread.ManagedThreadId);

            var ci = getInfo();
            for (var i = 0; i < ci.media.Count; i++)
            {
                if (ci.media[i].status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE &&
                    ci.media[i].type == pjmedia_type.PJMEDIA_TYPE_AUDIO)
                {
                    var aud_med = getAudioMedia(i);
                    if (aud_med == null)
                    {
                        _logger.LogWarning("Audio media is null for call {CallId} media index {MediaIndex}",
                            (int)base.getId(), i);
                        continue;
                    }

                    if (_mediaPort.getPortId() == pjsua2.INVALID_ID)
                    {
                        _logger.LogError("MediaPort is invalid for call {CallId}", (int)base.getId());
                        continue;
                    }

                    if (CallDirection == Direction.Incoming)
                    {
                        Console.WriteLine("Incoming call from " + ci.remoteUri);
                           if (!_hasSpoken){
                        _agent.Speak("Good afternoon, welcome to our store! My name is Petr, and I’m happy to assist you with your shopping. We offer a wide range of products, including food, drinks, and other goods. What would you like to buy today, or do you have any questions I can help you with?").Wait(); _hasSpoken = true;}
                        //       Task.Run(() => MediaPortTest.RunTest(_mediaPort, _logger, durationSeconds: 10));
                    }

                    try
                    {
                            // var speaker = Endpoint.instance().audDevManager().getPlaybackDevMedia();
                            // _mediaPort.startTransmit(speaker);
                            _logger.LogDebug("Starting transmission for call {CallId} on Thread-{ThreadId}",
                                (int)base.getId(), Thread.CurrentThread.ManagedThreadId);
                            aud_med.startTransmit(_mediaPort);
                            _mediaPort.startTransmit(aud_med);
                   
                        _logger.LogInformation("Audio transmission started for call {CallId} media index {MediaIndex}",
                            (int)base.getId(), i);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to start audio transmission for call {CallId} media index {MediaIndex}",
                            (int)base.getId(), i);
                    }
                }
            }
        }

        private void CleanupResources()
        {
            Console.WriteLine("Disposing");
            try
            {
                _mediaPortManager.RemoveMediaPort((int)base.getId());
                _mediaPort = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up resources for call {CallId}", (int)base.getId());
            }
        }

        public new void Dispose()
        {
            try
            {
                CleanupResources();

                base.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing call {CallId}", (int)base.getId());
            }
        }
    }
}