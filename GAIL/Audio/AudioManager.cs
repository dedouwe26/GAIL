using System.Numerics;
using GAIL.Core;
using Silk.NET.OpenAL;

namespace GAIL.Audio
{
    /// <summary>
    /// Handles all audio in the Application.
    /// </summary>
    public class AudioManager : IManager {
        public static List<string> GetAudioDevices() {
            unsafe {
                return [.. ALContext.GetApi().GetContextProperty(null, GetContextString.DeviceSpecifier).Split("\0")];
            }
        }
        /// <summary>
        /// OpenAL Context, for custom usage.
        /// </summary>
        public Pointer<Context> context;
        /// <summary>
        /// OpenAL Device, for custom usage.
        /// </summary>
        public Pointer<Device> device;
        public AudioManager() : this("") { }
        /// <summary>
        /// For a custom audio device selection.
        /// </summary>
        /// <param name="audioDevice">The custom selected audio device name. Get the name from <see cref="GetAudioDevices"/></param>
        /// <exception cref="APIBackendException"></exception>
        public AudioManager(string audioDevice) {
            ALContext alc = ALContext.GetApi();
            unsafe {
                device = alc.OpenDevice(audioDevice);
                if (device.IsNull) {
                    throw new APIBackendException("OpenAL", string.Format("Failed to open device ({0})", audioDevice));
                }
                context = alc.CreateContext(device, null);
                if (context.IsNull) {
                    throw new APIBackendException("OpenAL", "Failed to create OpenAL context.");
                }
            }
        }
        ~AudioManager() {
            Dispose();
        }
        /// <inheritdoc/>
        /// <exception cref="APIBackendException"></exception>
        public void Dispose() {
            ALContext alc = ALContext.GetApi();
            unsafe {
                if (!alc.MakeContextCurrent(null)) {
                    throw new APIBackendException("OpenAL", "Failed to deassign current context.");
                }
                alc.DestroyContext(context);
                if (!alc.CloseDevice(device)) {
                    throw new APIBackendException("OpenAL", "Failed to close device.");
                }
            }
        }
        /// <summary>
        /// Starts playing a sound. If the sound is already being played, it will restart from the beginning.
        /// <para/> If you call <see cref="Sound.Update()"/> it will break if the sound was playing or paused. <para/>
        /// See also: <seealso cref="PlaySound3D"/>.
        /// </summary>
        /// <param name="sound">The sound to play.</param>
        /// <exception cref="APIBackendException"></exception>
        public void PlaySound(Sound sound) {
            sound.Update();
            ALContext alc = ALContext.GetApi();
            AL al = AL.GetApi();
            unsafe {
                if (!alc.MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            al.SourcePlay(sound.source);
            int state = (int)SourceState.Playing;
            do {
                al.GetSourceProperty(sound.source, GetSourceInteger.SourceState, out state);
            } while (state==(int)SourceState.Playing);
            al.DeleteSource(sound.source);
            al.DeleteBuffer(sound.buffer);
        }
        /// <summary>
        /// Starts playing a sound with 3D positions. If the sound is already being played, it will restart from the beginning.
        /// <para/> If you call <see cref="Sound.Update()"/> it will break if the sound was playing or paused.
        /// </summary>
        /// <param name="sound">The sound to play.</param>
        /// <param name="position">The (start) position in 3D space.</param>
        /// <param name="velocity">The velocity of the movement.</param>
        /// <param name="direction">The direction of the movement.</param>
        /// <exception cref="APIBackendException"></exception>
        public void PlaySound3D(Sound sound, Vector3 position, Vector3 velocity, Vector3 direction) {
            sound.Update();
            AL al = AL.GetApi();
            unsafe {
                if (!ALContext.GetApi().MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            al.SetSourceProperty(sound.source, SourceVector3.Position, in position);
            al.SetSourceProperty(sound.source, SourceVector3.Velocity, in velocity);
            al.SetSourceProperty(sound.source, SourceVector3.Direction, in direction);
            
            al.SourcePlay(sound.source);
            int state;
            do {
                al.GetSourceProperty(sound.source, GetSourceInteger.SourceState, out state);
            } while (state!=(int)SourceState.Stopped);
            al.DeleteSource(sound.source);
            al.DeleteBuffer(sound.buffer);
        }
        /// <summary>
        /// Stops an playing or paused sound.
        /// </summary>
        /// <param name="sound">The sound to stop.</param>
        /// <exception cref="APIBackendException"></exception>
        public void StopSound(Sound sound) {
            unsafe {
                if (!ALContext.GetApi().MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            AL.GetApi().SourceStop(sound.source);
        }
        /// <summary>
        /// Pauses or resumes a sound.
        /// </summary>
        /// <param name="sound">The sound to pause or resume.</param>
        /// <exception cref="APIBackendException"></exception>
        public void PauseSound(Sound sound) {
            AL al = AL.GetApi();
            unsafe {
                if (!ALContext.GetApi().MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            al.GetSourceProperty(sound.source, GetSourceInteger.SourceState, out int state);
            if (state == (int)SourceState.Paused) { al.SourcePlay(sound.source); } else { al.SourcePause(sound.source); }
        }
        /// <summary>
        /// Checks if the sound is still playing or is paused.
        /// </summary>
        /// <param name="sound">The sound to check.</param>
        /// <returns>If the sound is still playing or is paused.</returns>
        /// <exception cref="APIBackendException"></exception>
        public (bool isPlaying, bool isPaused) GetSoundState(Sound sound) {
            AL al = AL.GetApi();
            unsafe {
                if (!ALContext.GetApi().MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            al.GetSourceProperty(sound.source, GetSourceInteger.SourceState, out int i);

            return (i==(int)SourceState.Playing, i==(int)SourceState.Paused);
        }
        /// <summary>
        /// Go's to the specified sample in the sound.
        /// </summary>
        /// <param name="sound">The sound where the sample offset is changed.</param>
        /// <param name="sample">The specified sample to go to.</param>
        /// <exception cref="APIBackendException"></exception>
        public void Goto(Sound sound, int sample) {
            ALContext alc = ALContext.GetApi();
            unsafe {
                if (!alc.MakeContextCurrent(context)) {
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            AL.GetApi().SetSourceProperty(sound.source, SourceInteger.SampleOffset, sample);

        }
        
    }
}