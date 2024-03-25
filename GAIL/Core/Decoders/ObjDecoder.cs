using GAIL.Graphics;

namespace GAIL.Core
{
    public static partial class Obj {
        public partial class Decoder : IDecoder<Mesh> {
            public void Dispose() {
                GC.SuppressFinalize(this);
            }
            /// <summary>
            /// Parses from a stream.
            /// </summary>
            /// <param name="stream">The stream to parse.</param>
            /// <returns>The parsed Mesh.</returns>
            public Mesh Parse(Stream stream) {
                throw new NotImplementedException();
            }
        }
    }
}