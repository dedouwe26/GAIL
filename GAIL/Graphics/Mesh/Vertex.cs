using System.Collections.ObjectModel;
using System.Numerics;
using GAIL.Graphics.Material;

namespace GAIL.Graphics.Mesh
{
    /// <summary>
    /// A point that exists of data (used for 3D / 2D objects).
    /// </summary>
    public class Vertex : IEquatable<Vertex> {
        /// <summary>
        /// Attributes for this vertex shown in shader.
        /// </summary>
        /// <remarks>
        /// The index of an attribute is used for the location in the vertex shader.
        /// </remarks>
        public List<VertexAttribute> Attributes { get; private set; }

        /// <summary>
        /// Creates a new vertex with a position of (0, 0, 0).
        /// </summary>
        public Vertex() {
            Attributes = [];
        }
        /// <summary>
        /// Creates a new vertex with a <see cref="PositionAttribute"/> at location 0.
        /// </summary>
        /// <param name="position">The position to set.</param>
        public Vertex(Vector3 position) : this() {
            Attributes.Add(new PositionAttribute(position));
        }

        /// <summary>
        /// Gives the nessecary vertex attributes to be used by the shader.
        /// </summary>
        /// <param name="requiredAttributes">The attribute types that are required by the shader.</param>
        /// <returns>The nessecary vertex attributes.</returns>
        /// <exception cref="NotSupportedException">This exception is fired when this vertex does not supply sufficient attributes.</exception>
        public VertexAttribute[] Supply(ReadOnlyCollection<AttributeType> requiredAttributes) {
            VertexAttribute[] result = new VertexAttribute[requiredAttributes.Count];
            int requiredIndex = 0;
            foreach (VertexAttribute attribute in Attributes) {
                if (attribute.type == requiredAttributes[requiredIndex]) {
                    result[requiredIndex] = attribute;
                    requiredIndex++;
                }
            }

            if (requiredIndex < requiredAttributes.Count) {
                throw new NotSupportedException("This vertex does not supply sufficient attributes.");
            }

            return result; 
        }

        /// <inheritdoc/>
        public bool Equals(Vertex? other)  {
            if (other == null) return false;

            if (ReferenceEquals(this, other)) return true;

            return Attributes.Equals(other.Attributes);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) { return Equals(obj as Vertex); }

        ///
        public static bool operator ==(Vertex? left, Vertex? right) {
            if (left is null && right is null) return true;
            if (left is null) return false;

            return left.Equals(right);
        }

        ///
        public static bool operator !=(Vertex? left, Vertex? right) {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override int GetHashCode() { return Attributes.GetHashCode(); }
    }
}