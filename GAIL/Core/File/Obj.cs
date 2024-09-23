using GAIL.Graphics.Mesh;

namespace GAIL.Core.File
{
    /// <summary>
    /// Can decode an OBJ file.
    /// </summary>
    public static partial class Obj {
        /// <summary>
        /// The OBJ decoder.
        /// </summary>
        public partial class Decoder : IDecoder<Mesh> { }
        /// <summary>
        /// Parses the file.
        /// </summary>
        /// <param name="path">The path to the Obj file.</param>
        /// <returns>The parsed mesh.</returns>
        public static Mesh Parse(string path) {
            throw new NotImplementedException();
        }
    }
}