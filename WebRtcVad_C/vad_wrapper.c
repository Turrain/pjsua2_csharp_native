#include "webrtcvad.h"
#include "common_audio/vad/include/webrtc_vad.h"
#include <stdint.h>

#if defined(_WIN32)
  #define EXPORT __declspec(dllexport)
#else
  #define EXPORT __attribute__((visibility("default")))
#endif

/*
 * Creates a new VAD instance.
 */
EXPORT VadHandle VadCreate(void) {
    return WebRtcVad_Create();
}

/*
 * Frees the memory allocated for a VAD instance.
 */
EXPORT void VadFree(VadHandle handle) {
    if (handle != NULL) {
        WebRtcVad_Free(handle);
    }
}

/*
 * Initializes a VAD instance with default settings.
 */
EXPORT int VadInit(VadHandle handle) {
    if (handle == NULL) {
        return VAD_ERROR;
    }
    return WebRtcVad_Init(handle);
}

/*
 * Sets the VAD operating mode (aggressiveness).
 */
EXPORT int VadSetMode(VadHandle handle, VadMode mode) {
    if (handle == NULL) {
        return VAD_ERROR;
    }
    return WebRtcVad_set_mode(handle, mode);
}

/*
 * Processes an audio frame to detect voice activity.
 */
EXPORT int VadProcess(VadHandle handle,
                      VadSampleRate sample_rate,
                      const int16_t* audio_frame,
                      size_t frame_length) {
    if (handle == NULL || audio_frame == NULL) {
        return VAD_ERROR;
    }
    return WebRtcVad_Process(handle, sample_rate, audio_frame, frame_length);
}

/*
 * Validates that the sample rate and frame length are supported.
 */
EXPORT int VadValidRateAndFrameLength(VadSampleRate sample_rate, size_t frame_length) {
    return WebRtcVad_ValidRateAndFrameLength(sample_rate, frame_length);
}
