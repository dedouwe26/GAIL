using Silk.NET.OpenAL;

namespace GAIL.Audio
{
    /// <summary>
    /// A sound format: how many channels and the bandwidth.
    /// </summary>
    public enum SoundFormat {
        /// <summary>
        /// 2 channels with 16-bit bandwidth each.
        /// </summary>
        Stereo16 = BufferFormat.Stereo16,
        /// <summary>
        /// 1 channel with 16-bit bandwidth.
        /// </summary>
        Mono16 = BufferFormat.Mono16,
        /// <summary>
        /// 2 channels with 8-bit bandwidth each.
        /// </summary>
        Stereo8 = BufferFormat.Stereo8,
        /// <summary>
        /// 1 channel with 8-bit bandwidth.
        /// </summary>
        Mono8 = BufferFormat.Mono8
    }
}