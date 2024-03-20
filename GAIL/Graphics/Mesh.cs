using GAIL.Core;

namespace GAIL.Graphics
{
    /// <summary>
    /// A 3D / 2D shape data.
    /// </summary>
    public class Mesh {
        /// <summary>
        /// Returns a mesh constructed from a Wavefront obj file
        /// The attributes for each vertex that can be added are:
        /// "normal": NormalAttribute
        /// "uv": UVAttribute
        /// </summary>
        /// <param name="path">The path where the mesh is.</param>
        /// <returns>The parsed Mesh.</returns>
        public static Mesh FromObj(string path) { return Obj.Parse(path); }

        /// <summary>
        /// (Face0-p1, Face0-p2, Face0-p3, Face1-p1, etc)
        /// </summary>
        public List<uint> indexFaces = [];

        /// <summary>
        /// The vertices for the <see cref="indexFaces"/>.
        /// </summary>
        public List<Vertex> vertices = [];

        /// <summary>
        /// Creates an mesh from faces.
        /// </summary>
        /// <param name="faces">The faces to create from.</param>
        public Mesh(List<Face> faces) {
            // TODO: parse faces to index faces.
        }
        /// <summary>
        /// Creates a mesh from index faces.
        /// </summary>
        /// <param name="vertices">The vertices for the indices.</param>
        /// <param name="indexFaces">The face indices.</param>
        public Mesh(List<Vertex> vertices, List<uint> indexFaces) {
            this.vertices = vertices;
            this.indexFaces = indexFaces;
        }

    }
}