using System.Numerics;
using GAIL.Graphics.Mesh;

namespace GAIL.Core.File
{
    public static partial class Obj {
        public partial class Decoder : IDecoder<Mesh> {
            /// <inheritdoc/>
            public void Dispose() {
                GC.SuppressFinalize(this);
            }
            private List<Vertex> vertices = [];
            private List<uint> faces = [];
            /// <summary>
            /// Parses from a stream.
            /// </summary>
            /// <param name="stream">The stream to parse.</param>
            /// <returns>The parsed Mesh.</returns>
            public Mesh Parse(Stream stream) {
                // TODO: Make this (Assimp?).
                using StreamReader t = new(stream);
                string? line = t.ReadLine();
                while (line != null) {
                    ParseLine(line);
                }
                return new Mesh([.. vertices], [.. faces]);
            }
            private string[] lineFragments;
            private void ParseLine(string line) {
                if (line[0] == '#') return;
                lineFragments = line.Split(' ');
                switch (lineFragments[0]) {
                    case "v":
                        vertices.Add(new(new Vector3(float.Parse(lineFragments[1]), float.Parse(lineFragments[2]), float.Parse(lineFragments[3]))));
                        break;
                    case "f":
                        faces.Add(uint.Parse(lineFragments[1])-1);
                        faces.Add(uint.Parse(lineFragments[2])-1);
                        faces.Add(uint.Parse(lineFragments[3])-1);
                        break;
                    default:
                        // NOTE: Should throw exception, but this is not finished.
                        return;
                }
            }
        }
    }
}