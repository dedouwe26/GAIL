namespace GAIL.Graphics
{
    /// <summary>
    /// A attribute (vertex buffer) in the shader, changes per vertex, from CPU to GPU (extend to use). <br/>
    /// The location in the shader is based on the list order in a vertex.
    /// </summary>
    public abstract class VertexAttribute : IEquatable<VertexAttribute> {
        /// <summary>
        /// The type of this attribute.
        /// </summary>
        public readonly AttributeType type;

        /// <summary>
        /// Creates a new vertex attribute.
        /// </summary>
        /// <param name="type">The type of the attribute.</param>
        protected VertexAttribute(AttributeType type) { this.type = type; }
        /// <inheritdoc/>
        public bool Equals(VertexAttribute? other) {
            if (other == null) { return false; }
            return type == other.type;
        }

        /// <summary>
        /// Generates the data for this attribute.
        /// </summary>
        /// <returns>The data for the vertex input attributes.</returns>
        public abstract byte[] Use();
        /// <inheritdoc/>
        public override bool Equals(object? obj) {
            if (obj == null) { return false; }
            if (obj.GetType() != typeof(VertexAttribute)) { return false; }
            return Equals((VertexAttribute)obj);
        }
        /// <inheritdoc/>
        public override int GetHashCode() { return (int)type; }
    }
}