using GAIL.Core;
using LambdaKit.Logging;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.EXT;

namespace GAIL.Audio
{
    /// <summary>
    /// Reads audio from a microphone or sound output.
    /// </summary>
    public class SoundCapture : IDisposable {
        /// <summary>
        /// The OpenAL context for custom usage.
        /// </summary>
        public readonly Pointer<Context> context;

        /// <summary>
        /// the sample rate of this sound capture.
        /// </summary>
        public readonly uint sampleRate;
        /// <summary>
        /// The format of this sound capture.
        /// </summary>
        public readonly SoundFormat format;
        /// <summary>
        /// The capture device, used for custom usage.
        /// </summary>
        public Pointer<Device> captureDevice;
        /// <summary>
        /// The openAL device, used for custom usage.
        /// </summary>
        public Pointer<Device> device;
        /// <summary>
        /// All the already captured sound waves.
        /// </summary>
        byte[] buffer = [];

        /// <summary>
        /// Gets all the capture devices' names.
        /// </summary>
        /// <returns>A list of all the capture devices.</returns>
        public static List<string> GetCaptureDevices() {
            unsafe {
                return [.. ALContext.GetApi().GetContextProperty(null, GetContextString.DeviceSpecifier).Split('\0')];
            }
            
        }
        /// <summary>
        /// Creates a new sound capture.
        /// </summary>
        /// <param name="format">The format in which to capture the sound.</param>
        /// <param name="sampleRate">The sample rate in which to capture the sound.</param>
        /// <param name="deviceName">Which capture device to use (get name at <see cref="GetCaptureDevices"/> <b>OR pass an empty string for the default</b>).</param>
        /// <exception cref="APIBackendException">OpenAL: Unable to get the Capture extension.</exception>
        public SoundCapture(SoundFormat format, uint sampleRate, string deviceName) {
            unsafe {
                device = API.Alc.OpenDevice(deviceName);
                context = API.Alc.CreateContext(device, null);
                if (!API.Alc.MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
                if (API.Alc.TryGetExtension(device, out Capture cap)) {
                    throw new APIBackendException("OpenAL", "Unable to get the Capture extension.");
                }
                captureDevice = cap.CaptureOpenDevice(deviceName, sampleRate, format, 4096);
            }
            this.format = format;
            this.sampleRate = sampleRate;

        }
        /// <summary></summary>
        ~SoundCapture() {
            Dispose();
        }
        /// <summary>
        /// Closes all the OpenAL devices
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="APIBackendException"></exception>
        public void Dispose() {
            buffer = [];
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
                if (API.Alc.TryGetExtension(device, out Capture cap)) {
                    throw new APIBackendException("OpenAL", "Unable to get the Capture extension.");
                }
                cap.CaptureCloseDevice(captureDevice);
                API.Alc.CloseDevice(device);

                if (!API.Alc.MakeContextCurrent(null)) {
                    throw new APIBackendException("OpenAL", "Failed to deassign current context.");
                }

                API.Alc.DestroyContext(context);
            }
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Starts capturing the audio.
        /// </summary>
        /// <exception cref="APIBackendException"/>
        public void Start() {
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
                if (API.Alc.TryGetExtension(device, out Capture cap)) {
                    throw new APIBackendException("OpenAL", "Unable to get the Capture extension.");
                }
                cap.CaptureStart(captureDevice);

                API.Alc.DestroyContext(context);
            }
        }
        /// <summary>
        /// The enum for capturing buffer
        /// </summary>
        public enum BufferFormat { }
        /// <summary>
        /// Stores all the already captured data into the buffer.
        /// </summary>
        /// <returns>The last captured wave.</returns>
        /// <exception cref="APIBackendException">OpenAL: Unable to get the Capture extension.</exception>
        public byte Capture() {
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
                if (API.Alc.TryGetExtension(device, out Capture cap)) {
                    throw new APIBackendException("OpenAL", "Unable to get the Capture extension.");
                }
                cap.CaptureStop(captureDevice);
                int remainingSamples = cap.GetAvailableSamples(captureDevice);
                
                if (remainingSamples > 0) {
                    buffer = [.. buffer, .. cap.CaptureSamples<byte, BufferFormat>(captureDevice, new BufferFormat(), remainingSamples)];
                }

                API.Alc.DestroyContext(context);
            }
            return buffer[^1];
        }

        /// <summary>
        /// Stops capturing the audio and returns the whole sound (including previous captures).
        /// </summary>
        /// <returns>The captured sound.</returns>
        /// <exception cref="APIBackendException">OpenAL: Unable to get the Capture extension.</exception>
        public Sound Stop() {
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
                if (API.Alc.TryGetExtension(device, out Capture cap)) {
                    throw new APIBackendException("OpenAL", "Unable to get the Capture extension.");
                }
                
                cap.CaptureStop(captureDevice);
                int remainingSamples = cap.GetAvailableSamples(captureDevice);
                
                if (remainingSamples > 0) {
                    buffer = [.. buffer, .. cap.CaptureSamples<byte, BufferFormat>(captureDevice, new BufferFormat(), remainingSamples)];
                }
            }
            return new Sound((int)sampleRate, format, [.. buffer]);
        }
    }
}