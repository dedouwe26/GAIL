using GAIL.Audio;

namespace GAIL.Core
{
    /// <summary>
    /// Can decode a WAV file.
    /// </summary>
    public static partial class WAV
    {
        /// <summary>
        /// The WAV decoder.
        /// </summary>
        public partial class Decoder : IDecoder<Sound> { }
        /// <summary>
        /// Parses the file.
        /// </summary>
        /// <param name="path">The path to the WAV file.</param>
        /// <returns>The parsed sound.</returns>
        public static Sound Parse(string path) {
            return new Decoder().Parse(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None));
        }
    }
}