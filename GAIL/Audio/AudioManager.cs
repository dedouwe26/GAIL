using System.Numerics;
using GAIL.Core;
using LambdaKit.Logging;
using Silk.NET.OpenAL;

namespace GAIL.Audio
{
    /// <summary>
    /// Handles all audio in the Application.
    /// </summary>
    public class AudioManager : IDisposable {
        /// <summary>
        /// If this manager is already disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// Gets all the audio devices names.
        /// </summary>
        /// <returns>A list of the names of all the audio devices.</returns>
        public static List<string> GetAudioDevices() {
            unsafe {
                return [.. ALContext.GetApi().GetContextProperty(Pointer<Device>.FromNull(), GetContextString.DeviceSpecifier).Split("\0")];
            }
        }
        /// <summary>
        /// OpenAL Context, for custom usage.
        /// </summary>
        public Pointer<Context> context = Pointer<Context>.FromNull();
        /// <summary>
        /// OpenAL Device, for custom usage.
        /// </summary>
        public Pointer<Device> device = Pointer<Device>.FromNull();

        /// <summary>
        /// The logger corresponding to the graphics part of the application.
        /// </summary>
        public readonly Logger Logger;

        /// <summary>
        /// Creates an audio manager. Use <see cref="Initialize"/> to initialize the manager.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public AudioManager(Logger logger) {
            Logger = logger;
        }

        /// <summary>
        /// Initializes the audio manager.
        /// </summary>
        /// <param name="audioDevice">The custom selected audio device name. Get the name from <see cref="GetAudioDevices"/> (default: default device).</param>
        /// <exception cref="APIBackendException"></exception>
        public void Initialize(string audioDevice = "") {
            unsafe {
                device = API.Alc.OpenDevice(audioDevice);
                if (device.IsNull) {
                    Logger.LogFatal($"OpenAL: Failed to open device ({audioDevice}).");
                    throw new APIBackendException("OpenAL", $"Failed to open device ({audioDevice}).");
                }
                context = API.Alc.CreateContext(device, null);
                if (context.IsNull) {
                    Logger.LogFatal($"OpenAL: Failed to create OpenAL context.");
                    throw new APIBackendException("OpenAL", "Failed to create OpenAL context.");
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="APIBackendException"></exception>
        public void Dispose() {
            if (IsDisposed) { return; }

            Logger.LogDebug("Terminating OpenAL.");
            unsafe {
                if (!API.Alc.MakeContextCurrent(null)) {
                    Logger.LogError("OpenAL: Failed to deassign current context.");
                    throw new APIBackendException("OpenAL", "Failed to deassign current context.");
                }
                API.Alc.DestroyContext(context);
                if (!API.Alc.CloseDevice(device)) {
                    Logger.LogError("OpenAL: Failed to close device.");
                    throw new APIBackendException("OpenAL", "Failed to close device.");
                }
            }

            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Starts playing a sound. If the sound is already being played, it will restart from the beginning.
        /// <para/> If you call <see cref="Sound.Update()"/> it will break if the sound was playing or paused. <para/>
        /// See also: <seealso cref="PlaySound3D"/>.
        /// </summary>
        /// <param name="sound">The sound to play.</param>
        /// <exception cref="APIBackendException"></exception>
        public void PlaySound(Sound sound) {
            Logger.LogDebug("Playing sound.");
            sound.Update();
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    Logger.LogError("OpenAL: Failed to make context current.");
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            API.Al.SourcePlay(sound.source);
            int state;
            do {
                API.Al.GetSourceProperty(sound.source, GetSourceInteger.SourceState, out state);
            } while (state==(int)SourceState.Playing);
            
            API.Al.DeleteSource(sound.source);
            API.Al.DeleteBuffer(sound.buffer);
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
            Logger.LogDebug("Playing sound in 3D.");
            sound.Update();
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    Logger.LogError("OpenAL: Failed to make context current.");
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            API.Al.SetSourceProperty(sound.source, SourceVector3.Position, in position);
            API.Al.SetSourceProperty(sound.source, SourceVector3.Velocity, in velocity);
            API.Al.SetSourceProperty(sound.source, SourceVector3.Direction, in direction);
            
            API.Al.SourcePlay(sound.source);
            int state;
            do {
                API.Al.GetSourceProperty(sound.source, GetSourceInteger.SourceState, out state);
            } while (state!=(int)SourceState.Stopped);
            API.Al.DeleteSource(sound.source);
            API.Al.DeleteBuffer(sound.buffer);
        }
        /// <summary>
        /// Stops an playing or paused sound.
        /// </summary>
        /// <param name="sound">The sound to stop.</param>
        /// <exception cref="APIBackendException"></exception>
        public void StopSound(Sound sound) {
            Logger.LogDebug("Stopping sound.");
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    Logger.LogError("OpenAL: Failed to make context current.");
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            API.Al.SourceStop(sound.source);
        }
        /// <summary>
        /// Pauses or resumes a sound.
        /// </summary>
        /// <param name="sound">The sound to pause or resume.</param>
        /// <exception cref="APIBackendException"></exception>
        public void PauseSound(Sound sound) {
            Logger.LogDebug("Pausing sound.");
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    Logger.LogError("OpenAL: Failed to make context current.");
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            API.Al.GetSourceProperty(sound.source, GetSourceInteger.SourceState, out int state);
            if (state == (int)SourceState.Paused) { API.Al.SourcePlay(sound.source); } else { API.Al.SourcePause(sound.source); }
        }
        /// <summary>
        /// Changes the 3D stuff of a sound (like position).
        /// </summary>
        /// <param name="sound">The sound to change the 3D stuff.</param>
        /// <param name="position">The (start) position in 3D space.</param>
        /// <param name="velocity">The velocity of the movement.</param>
        /// <param name="direction">The direction of the movement.</param>
        /// <exception cref="APIBackendException"></exception>
        public void SetSound3D(Sound sound, Vector3 position, Vector3 velocity, Vector3 direction) {
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    Logger.LogError("OpenAL: Failed to make context current.");
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            API.Al.SetSourceProperty(sound.source, SourceVector3.Position, in position);
            API.Al.SetSourceProperty(sound.source, SourceVector3.Velocity, in velocity);
            API.Al.SetSourceProperty(sound.source, SourceVector3.Direction, in direction);
        }
        /// <summary>
        /// Checks if the sound is still playing or is paused.
        /// </summary>
        /// <param name="sound">The sound to check.</param>
        /// <returns>If the sound is still playing or is paused.</returns>
        /// <exception cref="APIBackendException"></exception>
        public (bool isPlaying, bool isPaused) GetSoundState(Sound sound) {
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    Logger.LogError("OpenAL: Failed to make context current.");
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            API.Al.GetSourceProperty(sound.source, GetSourceInteger.SourceState, out int i);

            return (i==(int)SourceState.Playing, i==(int)SourceState.Paused);
        }
        /// <summary>
        /// Go's to the specified sample in the sound.
        /// </summary>
        /// <param name="sound">The sound where the sample offset is changed.</param>
        /// <param name="sample">The specified sample to go to.</param>
        /// <exception cref="APIBackendException"></exception>
        public void Goto(Sound sound, int sample) {
            Logger.LogDebug("Going to sample.");
            unsafe {
                if (!API.Alc.MakeContextCurrent(context)) {
                    Logger.LogError("OpenAL: Failed to make context current.");
                    throw new APIBackendException("OpenAL", "Failed to make context current.");
                }
            }
            API.Al.SetSourceProperty(sound.source, SourceInteger.SampleOffset, sample);

        }
    }
}