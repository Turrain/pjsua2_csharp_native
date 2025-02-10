/*
 * WebRTC Voice Activity Detection (VAD) wrapper
 */

#ifndef WEBRTC_VAD_H_
#define WEBRTC_VAD_H_

#include <stddef.h>
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

#if defined(_WIN32)
    #define EXPORT __declspec(dllexport)
#else
    #define EXPORT __attribute__((visibility("default")))
#endif

/* Handle to a VAD instance */
typedef void* VadHandle;

/* Return codes */
#define VAD_SUCCESS 0
#define VAD_ERROR -1

/* VAD operating modes (aggressiveness) */
typedef enum {
    VAD_MODE_NORMAL = 0,         /* Normal detection mode */
    VAD_MODE_LOW_BITRATE = 1,      /* More aggressive noise reduction */
    VAD_MODE_AGGRESSIVE = 2,       /* Even more aggressive noise reduction */
    VAD_MODE_VERY_AGGRESSIVE = 3   /* Highest sensitivity to noise */
} VadMode;

/* Supported sample rates */
typedef enum {
    VAD_SAMPLE_RATE_8KHZ = 8000,
    VAD_SAMPLE_RATE_16KHZ = 16000,
    VAD_SAMPLE_RATE_32KHZ = 32000
} VadSampleRate;

/*
 * Creates and returns a new VAD instance.
 *
 * Returns: A valid VAD instance handle if successful, NULL otherwise.
 */
EXPORT VadHandle VadCreate(void);

/*
 * Frees the memory allocated for a VAD instance.
 *
 * handle: The VAD instance to be freed.
 */
EXPORT void VadFree(VadHandle handle);

/*
 * Initializes a VAD instance with default settings.
 *
 * handle: The VAD instance to initialize.
 *
 * Returns: VAD_SUCCESS on success, VAD_ERROR on failure.
 */
EXPORT int VadInit(VadHandle handle);

/*
 * Sets the VAD operating mode (aggressiveness).
 *
 * handle: The VAD instance.
 * mode: The desired aggressiveness mode.
 *
 * Returns: VAD_SUCCESS on success, VAD_ERROR on failure.
 */
EXPORT int VadSetMode(VadHandle handle, VadMode mode);

/*
 * Processes an audio frame to detect voice activity.
 *
 * handle: The VAD instance.
 * sample_rate: Sampling frequency in Hz (8000, 16000, or 32000).
 * audio_frame: Buffer containing 16-bit PCM audio samples.
 * frame_length: Number of samples in the audio frame.
 *
 * Returns:
 *   1 if voice activity is detected,
 *   0 if no voice is detected,
 *  -1 if an error occurs.
 */
EXPORT int VadProcess(VadHandle handle,
                      VadSampleRate sample_rate,
                      const int16_t* audio_frame,
                      size_t frame_length);

/*
 * Validates the combination of sample rate and frame length.
 * (Supported frame lengths correspond to 10, 20, or 30 ms of audio.)
 *
 * sample_rate: Sampling frequency in Hz.
 * frame_length: Number of samples in the frame.
 *
 * Returns: VAD_SUCCESS if valid, VAD_ERROR if invalid.
 */
EXPORT int VadValidRateAndFrameLength(VadSampleRate sample_rate, size_t frame_length);

#ifdef __cplusplus
}
#endif

#endif /* WEBRTC_VAD_H_ */
