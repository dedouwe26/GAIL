namespace GAIL.Core
{
    public static partial class PNG {
        public partial class Decoder : IDecoder<Texture> {
            /// <inheritdoc/>
            public void Dispose() {
                GC.SuppressFinalize(this);
            }
            /// <summary>
            /// Parses from a stream.
            /// </summary>
            /// <param name="stream">The stream to parse.</param>
            /// <returns>The parsed Texture.</returns>
            public Texture Parse(Stream stream) {
                throw new NotImplementedException();
            }
        }
    }
}