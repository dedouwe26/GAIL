using System.Numerics;

namespace GAIL.Graphics
{
    /// <summary>
    /// A point that exists of data (used for 3D / 2D objects).
    /// </summary>
    public class Vertex : IEquatable<Vertex>
    {
        /// <summary>
        /// The position of this vertex.
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// Attributes for this vertex shown in shader.
        /// Name and VertexAttribute.
        /// </summary>
        public Dictionary<string, VertexAttribute> attributes = [];

        /// <summary>
        /// Creates a new vertex with a position of (0, 0, 0).
        /// </summary>
        public Vertex() { Position = Vector3.Zero; }
        /// <summary>
        /// Creates a new vertex with a position.
        /// </summary>
        /// <param name="position">The position to set.</param>
        public Vertex(Vector3 position) { Position = position; }
        /// <summary>
        /// Create a new vertex with the x, y, z coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">the z coordinate.</param>
        public Vertex(float x, float y, float z) { Position = new Vector3(x, y, z); }
        /// <summary>
        /// Create a new vertex with the x, y coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Vertex(float x, float y) { Position = new Vector3(x, y, 0); }

        /// <summary>
        /// Adds an attribute to the vertex.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="attribute">The attribute itself.</param>
        public void AddAttribute(string name, VertexAttribute attribute) { attributes.Add(name, attribute); }

        public bool Equals(Vertex? other)  {
            if (other == null) { return false; }
            return Position == other.Position && attributes == other.attributes;
        }

        public override bool Equals(object? obj) { return Equals(obj as Vertex); }

        public override int GetHashCode() { return Position.GetHashCode(); }
    }
}