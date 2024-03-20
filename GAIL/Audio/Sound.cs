using GAIL.Core;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.EXT;

namespace GAIL.Audio
{
    /// <summary>
    /// Represents a sound (audio) with effects.
    /// </summary>
    public class Sound : IDisposable {
        /// <param name="sampleRate"><example>example: 44100(Hz)</example></param>
        /// <param name="format">The format of the rawData.</param>
        /// <param name="rawData">Raw sound data is in PCM</param>
        public Sound(int sampleRate, SoundFormat format, List<byte> rawData) {
            al = AL.GetApi();
            
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
        ~Sound() {
            Dispose();
        }
        public void Dispose() {
            al.DeleteSource(source);
            al.DeleteBuffer(buffer);
            soundEffects.Clear();
            rawData.Clear();

            al.Dispose();
        }
        /// <summary>
        /// The OpenAL API instance for custom usage.
        /// </summary>
        public readonly AL al;

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
            return WAV.Parse(path);
        }
        
        
    }
    
}