using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PjSua2.Native.pjsua2;
using PjSip.App.Services;
using PjSip.App.Exceptions;
using PjSip.App.Data;

namespace PjSip.App.Sip
{
    public class Call : PjSua2.Native.pjsua2.Call, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly AgentManager _agentManager;
        private readonly ILogger<Call> _logger;
        public MediaPort MediaPort { get; private set; }

        public enum Direction { Incoming, Outgoing }
        public Direction CallDirection { get; set; }

        public Call(Account acc,int callId, ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
             : base(acc, callId) 
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = loggerFactory.CreateLogger<Call>();

            // Use a new scope to obtain a fresh SipDbContext instance for AgentManager initialization.
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();
                try
                {
                    MediaPort = new MediaPort();
                    _agentManager = new AgentManager(context, loggerFactory.CreateLogger<AgentManager>());
                    _agentManager.RegisterMediaPort((int)base.getId(), MediaPort);
                    _logger.LogInformation("Call {CallId} initialized successfully", (int)base.getId());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize call {CallId}", (int)base.getId());
                    throw new SipCallException("Failed to initialize call", (int)base.getId(), "INIT_FAILED", ex);
                }
            }
        }

        public override void onCallState(OnCallStateParam prm)
        {
            try
            {
                var info = base.getInfo();
                var state = info.stateText;
                _logger.LogInformation("Call {CallId} state changed to {State}", (int)base.getId(), state);

                // Handle call termination.
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
    _logger.LogInformation("Call {CallId} media state changed", (int)base.getId());
    // try
    // {
    //     var info = base.getInfo();
    //     foreach (var media in info.media)
    //     {
    //         if (media.type == pjmedia_type.PJMEDIA_TYPE_AUDIO &&
    //             media.status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE)
    //         {
    //             try
    //             {
    //                 // Get the index of the first active audio media
    //                 int mediaIdx = -1;
    //                 for (int i = 0; i < info.media.Count; i++)
    //                 {
    //                     if (info.media[i].type == pjmedia_type.PJMEDIA_TYPE_AUDIO)
    //                     {
    //                         mediaIdx = i;
    //                         break;
    //                     }
    //                 }

    //                 if (mediaIdx >= 0)
    //                 {
    //                     var aud = base.getAudioMedia(mediaIdx);
    //                     if (aud != null && MediaPort != null)
    //                     {
    //                         aud.startTransmit(MediaPort);
    //                         MediaPort.startTransmit(aud);
    //                         _logger.LogInformation("Audio media started for call {CallId} using media index {MediaIdx}", 
    //                             (int)base.getId(), mediaIdx);
    //                     }
    //                     else
    //                     {
    //                         _logger.LogWarning("Audio media or MediaPort is null for call {CallId}", (int)base.getId());
    //                     }
    //                 }
    //                 else
    //                 {
    //                     _logger.LogWarning("No valid audio media index found for call {CallId}", (int)base.getId());
    //                 }
    //             }
    //             catch (Exception ex)
    //             {
    //                 _logger.LogError(ex, "Failed to start audio media for call {CallId}", (int)base.getId());
    //                 throw new MediaOperationException("Failed to start audio transmission", (int)base.getId(), "START_MEDIA", ex);
    //             }
    //         }
    //     }
    // }
    // catch (Exception ex)
    // {
    //     _logger.LogError(ex, "Error handling media state change for {CallId}", (int)base.getId());
    // }
}

        private void CleanupResources()
        {
            try
            {
                MediaPort?.Dispose();
                MediaPort = null;
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
                _agentManager?.Dispose();
                base.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing call {CallId}", (int)base.getId());
            }
        }
    }
}