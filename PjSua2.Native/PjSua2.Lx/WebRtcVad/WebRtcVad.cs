using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace PjSua2.Lx.WebRtcVad
{
    /// <summary>
    /// Enum for VAD aggressiveness modes.
    /// </summary>
    public enum VadMode
    {
        Normal = 0,         // VAD_MODE_NORMAL
        LowBitrate = 1,     // VAD_MODE_LOW_BITRATE
        Aggressive = 2,     // VAD_MODE_AGGRESSIVE
        VeryAggressive = 3, // VAD_MODE_VERY_AGGRESSIVE
    }

    /// <summary>
    /// Enum for supported sample rates.
    /// </summary>
    public enum VadSampleRate
    {
        Rate8KHz = 8000,    // VAD_SAMPLE_RATE_8KHZ
        Rate16KHz = 16000,  // VAD_SAMPLE_RATE_16KHZ
        Rate32KHz = 32000,  // VAD_SAMPLE_RATE_32KHZ
    }

    /// <summary>
    /// Contains the P/Invoke declarations for the native WebRTC VAD library.
    /// </summary>
    internal static class NativeMethods
    {
        // Update this constant if your shared library file is named differently.
        private const string LIB_NAME = "libwebrtcvad";

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr VadCreate();

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void VadFree(IntPtr handle);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VadInit(IntPtr handle);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VadSetMode(IntPtr handle, VadMode mode);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VadProcess(IntPtr handle,
                                            VadSampleRate sample_rate,
                                            [In] short[] audio_frame,
                                            UIntPtr frame_length);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VadValidRateAndFrameLength(VadSampleRate sample_rate, UIntPtr frame_length);
    }

    /// <summary>
    /// A SafeHandle implementation for the native VAD instance.
    /// </summary>
    internal sealed class VadSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private VadSafeHandle() : base(true)
        {
        }

        /// <summary>
        /// Creates a new safe handle wrapping a native VAD instance.
        /// </summary>
        /// <returns>A valid <see cref="VadSafeHandle"/>.</returns>
        public static VadSafeHandle Create()
        {
            IntPtr handle = NativeMethods.VadCreate();
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create VAD instance.");
            }
            return new VadSafeHandle() { handle = handle };
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                NativeMethods.VadFree(handle);
            }
            return true;
        }
    }

    /// <summary>
    /// A managed wrapper for the WebRTC Voice Activity Detection (VAD) functionality.
    /// This class supports processing audio frames passed as both <see cref="short[]"/> and <see cref="byte[]"/>.
    /// </summary>
    public sealed class WebRtcVad : IDisposable
    {
        private VadSafeHandle _handle;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRtcVad"/> class.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the native instance cannot be created or initialized.</exception>
        public WebRtcVad()
        {
            _handle = VadSafeHandle.Create();

            int initResult = NativeMethods.VadInit(_handle.DangerousGetHandle());
            if (initResult != 0)
            {
                _handle.Dispose();
                throw new InvalidOperationException("Failed to initialize VAD instance.");
            }
        }

        /// <summary>
        /// Sets the VAD operating mode (aggressiveness).
        /// </summary>
        /// <param name="mode">The desired VAD mode.</param>
        /// <exception cref="InvalidOperationException">Thrown if setting the mode fails.</exception>
        public void SetMode(VadMode mode)
        {
            EnsureNotDisposed();
            int result = NativeMethods.VadSetMode(_handle.DangerousGetHandle(), mode);
            if (result != 0)
            {
                throw new InvalidOperationException("Failed to set VAD mode.");
            }
        }

        /// <summary>
        /// Processes an audio frame (provided as an array of 16-bit PCM samples) to detect voice activity.
        /// </summary>
        /// <param name="sampleRate">The sample rate of the audio.</param>
        /// <param name="audioFrame">An array containing the 16-bit PCM audio samples.</param>
        /// <returns>
        /// 1 if voice is detected,
        /// 0 if no voice is detected,
        /// -1 if an error occurred.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="audioFrame"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the combination of sample rate and frame length is invalid or the frame is empty.</exception>
        public int Process(VadSampleRate sampleRate, short[] audioFrame)
        {
            EnsureNotDisposed();
            if (audioFrame == null)
            {
                throw new ArgumentNullException(nameof(audioFrame));
            }
            if (audioFrame.Length == 0)
            {
                throw new ArgumentException("Audio frame cannot be empty.", nameof(audioFrame));
            }

            UIntPtr frameLength = (UIntPtr)audioFrame.Length;

            // Validate the combination of sample rate and frame length.
            int valid = NativeMethods.VadValidRateAndFrameLength(sampleRate, frameLength);
            if (valid != 0)
            {
                throw new ArgumentException("Invalid combination of sample rate and frame length.", nameof(audioFrame));
            }

            return NativeMethods.VadProcess(_handle.DangerousGetHandle(), sampleRate, audioFrame, frameLength);
        }

        /// <summary>
        /// Processes an audio frame provided as a byte array (raw 16-bit PCM data) to detect voice activity.
        /// </summary>
        /// <param name="sampleRate">The sample rate of the audio.</param>
        /// <param name="audioFrame">
        /// A byte array containing the raw 16-bit PCM audio data.
        /// The length must be even.
        /// </param>
        /// <returns>
        /// 1 if voice is detected,
        /// 0 if no voice is detected,
        /// -1 if an error occurred.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="audioFrame"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the byte array length is not even or if the combination of sample rate and frame length is invalid.
        /// </exception>
        public int Process(VadSampleRate sampleRate, byte[] audioFrame)
        {
            EnsureNotDisposed();
            if (audioFrame == null)
            {
                throw new ArgumentNullException(nameof(audioFrame));
            }
            if (audioFrame.Length % 2 != 0)
            {
                throw new ArgumentException("Byte array length must be even to represent 16-bit PCM samples.", nameof(audioFrame));
            }

            int sampleCount = audioFrame.Length / 2;
            short[] samples = new short[sampleCount];
            Buffer.BlockCopy(audioFrame, 0, samples, 0, audioFrame.Length);
            return Process(sampleRate, samples);
        }

        /// <summary>
        /// Disposes of the native resources associated with the VAD instance.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _handle?.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if this instance has been disposed.
        /// </summary>
        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(WebRtcVad));
            }
        }
    }
}
