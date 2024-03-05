using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.EXT;

namespace GAIL
{
    /// <summary>
    /// A sound format: how many channels and the bandwidth.
    /// </summary>
    public enum SoundFormat {
        Stereo16 = BufferFormat.Stereo16,
        Mono16 = BufferFormat.Mono16,
        Stereo8 = BufferFormat.Stereo8,
        Mono8 = BufferFormat.Mono8
    }
    /// <summary>
    /// An effect template for sounds.
    /// </summary>
    public interface ISoundEffect {
        /// <summary>
        /// Applies effects to the sound.
        /// </summary>
        /// <param name="baseSound">The original sound.</param>
        /// <returns>The effected sound.</returns>
        Sound Use(Sound baseSound);
    }
    /// <summary>
    /// Represents a sound (audio) with effects.
    /// </summary>
    public class Sound : IDisposable {
        /// <param name="sampleRate"><example>example: 44100(Hz)</example></param>
        /// <param name="format">The format of the rawData.</param>
        /// <param name="rawData">Raw sound data is in PCM</param>
        public Sound(int sampleRate, SoundFormat format, List<byte> rawData) {
            AL al = AL.GetApi();
            
            buffer = al.GenBuffer();

            Sound sound = this;
            foreach (ISoundEffect effect in soundEffects) {
                sound = effect.Use(sound);
            }

            al.BufferData(buffer, (BufferFormat)format, rawData.ToArray(), sampleRate);
            source = al.GenSource();
            al.SetSourceProperty(source, SourceFloat.Pitch, pitch);
            al.SetSourceProperty(source, SourceFloat.Gain, volume);
            al.SetSourceProperty(source, SourceBoolean.Looping, isLooping);
            al.SetSourceProperty(source, SourceInteger.Buffer, buffer);
        }
        public void Dispose() {
            AL al = AL.GetApi();
            al.DeleteSource(source);
            al.DeleteBuffer(buffer);
            soundEffects.Clear();
            rawData.Clear();
        }

        /// <summary>
        /// The format of this sound.
        /// </summary>
        public readonly SoundFormat format;
        /// <summary>
        /// The duration of this sound (in seconds).
        /// </summary>
        public double Duration {get {
            return rawData.Count / (format == SoundFormat.Mono16||format == SoundFormat.Stereo16? 2 : 1) / sampleRate;
        }}
        /// <summary>
        /// The sample rate of this sound (in Hertz).
        /// </summary>
        public readonly int sampleRate;
        /// <summary>
        /// The raw sound data (when the bandwidth is 16 bits, 2 bytes per sample).
        /// </summary>
        public List<byte> rawData = [];
        /// <summary>
        // If this sound loops (default: false).
        /// </summary>
        public bool isLooping = false;
        /// <summary>
        /// The volume of this sound (default: 1).
        /// </summary>
        public float volume = 1;
        /// <summary>
        /// The pitch of this sound (default: 1).
        /// </summary>
        public float pitch = 1;
        /// <summary>
        /// The source (openAL), for custom usage.
        /// </summary>
        public uint source;
        /// <summary>
        /// The buffer (openAL), for custom usage.
        /// </summary>
        public uint buffer;

        /// <summary>
        /// All of the current sound effects.
        /// </summary>
        List<ISoundEffect> soundEffects = [];
        
        /// <summary>
        /// Updates the current source and buffer (call this when you use OpenAL).
        /// Also Applies all the sound effects (from first to last).
        /// </summary>
        public void Update() {
            AL al = AL.GetApi();

            al.DeleteSource(source);
            al.DeleteBuffer(buffer);
            
            buffer = al.GenBuffer();

            Sound sound = this;
            foreach (ISoundEffect effect in soundEffects) {
                sound = effect.Use(sound);
            }

            al.BufferData(buffer, (BufferFormat)format, rawData.ToArray(), sampleRate);
            source = al.GenSource();
            al.SetSourceProperty(source, SourceFloat.Pitch, pitch);
            al.SetSourceProperty(source, SourceFloat.Gain, volume);
            al.SetSourceProperty(source, SourceBoolean.Looping, isLooping);
            al.SetSourceProperty(source, SourceInteger.Buffer, buffer);
        }
        /// <summary>
        /// Sets the sound effects for later use.
        /// </summary>
        /// <param name="soundEffects">The sound effects</param>
        public void SetSoundEffects(List<ISoundEffect> soundEffects) { this.soundEffects = soundEffects; }
        /// <summary>
        /// Loads a sound from a wav file with a path to it.
        /// </summary>
        /// <param name="path">The path of the WAVE formatted file</param>
        /// <returns></returns>
        public static Sound FromWAV(string path) {
            using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            if (buffer != "RIFF".ToCharArray().Select(c => (byte)c)) {
                throw new FileFormatException(path, "RIFF header doesn't match");
            }
            stream.Read(buffer, 8, 4);
            if (buffer != "WAVE".ToCharArray().Select(c => (byte)c)) {
                throw new FileFormatException(path, "file format isn't WAVE");
            }
            stream.Read(buffer, 12, 4);
            if (buffer != "fmt ".ToCharArray().Select(c => (byte)c)) {
                throw new FileFormatException(path, "'fmt ' header doesn't match");
            }
            stream.Read(buffer, 22, 2); // num channels
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            short channels = BitConverter.ToInt16(buffer, 0);
            stream.Read(buffer, 24, 4); // Sample rate
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            int sampleRate = BitConverter.ToInt32(buffer, 0);
            stream.Read(buffer, 34, 2); // BitsPerSample
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            short bitsPerSample = BitConverter.ToInt16(buffer, 0);

            stream.Read(buffer, 36, 4);
            if (buffer != "data".ToCharArray().Select(c => (byte)c)) {
                throw new FileFormatException(path, "data header doesn't match");
            }
            stream.Read(buffer, 0, 4);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            int size = BitConverter.ToInt32(buffer, 0);
            byte[] data = new byte[size]; 
            stream.Read(data, 44, size);

            return new(sampleRate, channels==1&&bitsPerSample==8?SoundFormat.Mono8:channels==2&&bitsPerSample==8?SoundFormat.Stereo8:channels==1&&bitsPerSample==16?SoundFormat.Mono16:channels==2&&bitsPerSample==16?SoundFormat.Stereo16:SoundFormat.Stereo16, [.. data]);
        }
        /// <summary>
        /// Reads audio from a microphone or sound output.
        /// </summary>
        public class SoundCapture : IDisposable {
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
            /// Gets all the capture devices' names.
            /// </summary>
            /// <returns>A list of all the capture devices.</returns>
            public static unsafe List<string> GetCaptureDevices() {
                return [.. ALContext.GetApi().GetContextProperty(null, GetContextString.DeviceSpecifier).Split('\0')];
            }
            /// <summary>
            /// Creates a new sound capture.
            /// </summary>
            /// <param name="format">The format in which to capture the sound.</param>
            /// <param name="sampleRate">The sample rate in which to capture the sound.</param>
            /// <param name="deviceName">Which capture device to use. (Get name at <see cref="GetCaptureDevices"/> <b>OR pass an empty string for the default</b>)</param>
            public unsafe SoundCapture(SoundFormat format, uint sampleRate, string deviceName) {
                ALContext alc = ALContext.GetApi();
                device = alc.OpenDevice(deviceName);
                Pointer<Context> context = alc.CreateContext(device, null);
                alc.MakeContextCurrent(context);
                if (alc.TryGetExtension(device, out Capture cap)) {
                    throw new APIBackendException("OpenAL", "Unable to get the Capture extension.");
                }
                captureDevice = cap.CaptureOpenDevice(deviceName, sampleRate, format, 4096);
                this.format = format;
                this.sampleRate = sampleRate;

            }
            public unsafe void Dispose() {
                ALContext alc = ALContext.GetApi();
                Pointer<Context> context = alc.CreateContext(device, null);
                alc.MakeContextCurrent(context);
                if (alc.TryGetExtension(device, out Capture cap)) {
                    throw new APIBackendException("OpenAL", "Unable to get the Capture extension.");
                }
                cap.CaptureCloseDevice(captureDevice);
                alc.CloseDevice(device);
            }
            /// <summary>
            /// Starts capturing the audio.
            /// </summary>
            public void Start() {
                ALContext alc = ALContext.GetApi();
                unsafe {
                    Pointer<Context> context = alc.CreateContext(device, null);
                    alc.MakeContextCurrent(context);
                    if (alc.TryGetExtension(device, out Capture cap)) {
                        throw new APIBackendException("OpenAL", "Unable to get the Capture extension.");
                    }
                    cap.CaptureStart(captureDevice);
                }
            }
            /// <summary>
            /// Stops capturing the audio and returns the sound.
            /// </summary>
            /// <returns>The captured sound.</returns>
            public Sound Stop() {
                ALContext alc = ALContext.GetApi();
                unsafe {
                    Pointer<Context> context = alc.CreateContext(device, null);
                    alc.MakeContextCurrent(context);
                    if (alc.TryGetExtension(device, out Capture cap)) {
                        throw new APIBackendException("OpenAL", "Unable to get the Capture extension.");
                    }
                    List<byte> buffer = [];
                    cap.CaptureStop(captureDevice);
                    int remainingSamples = cap.GetAvailableSamples(captureDevice);
                    if (remainingSamples > 0) {
                        cap.CaptureSamples();
                    }
                }
            }
        }
    }
}