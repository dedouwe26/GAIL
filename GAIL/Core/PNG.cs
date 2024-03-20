
namespace GAIL.Core
{
    /// <summary>
    /// Can decode a PNG file.
    /// </summary>
    public static partial class PNG {
        /// <summary>
        /// The PNG decoder.
        /// </summary>
        public partial class Decoder : IDecoder<Texture> { }
        /// <summary>
        /// Parses the file.
        /// </summary>
        /// <param name="path">The path to the PNG file.</param>
        /// <returns>The parsed texture.</returns>
        public static Texture Parse(string path) {
            return new Decoder().Parse(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None));
        }
    }
}