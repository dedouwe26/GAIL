using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Numerics;

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
        public VertexAttribute[] attributes;

        /// <summary>
        /// Creates an empty vertex.
        /// </summary>
        public Vertex() {
            attributes = [];
        }
        /// <summary>
        /// Creates an vertex with the following attributes.
        /// </summary>
        /// <param name="attributes">The attributes to add.</param>
        public Vertex(VertexAttribute[] attributes) {
            this.attributes = attributes;
        }
        /// <summary>
        /// Creates a new vertex with a <see cref="PositionAttribute"/> at location 0.
        /// </summary>
        /// <param name="position">The position to set.</param>
        public Vertex(Vector3 position) {
            attributes = [new PositionAttribute(position)];
        }

        /// <summary>
        /// Gives the nessecary vertex attributes to be used by the shader.
        /// </summary>
        /// <param name="requiredAttributes">The attribute types that are required by the shader.</param>
        /// <returns>The nessecary vertex attributes.</returns>
        /// <exception cref="NotSupportedException">This exception is fired when this vertex does not supply sufficient attributes.</exception>
        public VertexAttribute[] Supply(ImmutableArray<FormatInfo> requiredAttributes) {
            VertexAttribute[] result = new VertexAttribute[requiredAttributes.Length];
            int requiredIndex = 0;
            foreach (VertexAttribute attribute in attributes) {
                if (attribute.info.type == requiredAttributes[requiredIndex].type) {
                    result[requiredIndex] = attribute;
                    requiredIndex++;
                }
            }

            if (requiredIndex < requiredAttributes.Length) {
                throw new NotSupportedException("This vertex does not supply sufficient attributes.");
            }

            return result; 
        }

        /// <inheritdoc/>
        public bool Equals(Vertex? other)  {
            if (other == null) return false;

            if (ReferenceEquals(this, other)) return true;

            return attributes.SequenceEqual(other.attributes);
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
        public override int GetHashCode() { return attributes.GetHashCode(); }
    }
}