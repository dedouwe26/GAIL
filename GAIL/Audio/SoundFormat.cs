using Silk.NET.OpenAL;

namespace GAIL.Audio
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
}