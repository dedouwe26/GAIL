namespace GAIL.Graphics.Mesh
{
    /// <summary>
    /// A face (triangle) of a mesh.
    /// </summary>
    /// <param name="p1">Point 1.</param>
    /// <param name="p2">Point 2.</param>
    /// <param name="p3">Point 3.</param>
    public class Face(Vertex p1, Vertex p2, Vertex p3) : IEquatable<Face> {
        /// <summary>
        /// Point 1 in this face.
        /// </summary>
        public Vertex p1 = p1;
        /// <summary>
        /// Point 2 in this face.
        /// </summary>
        public Vertex p2 = p2;
        /// <summary>
        /// Point 3 in this face.
        /// </summary>
        public Vertex p3 = p3;

        /// <inheritdoc/>
        public bool Equals(Face? other)  {
            if (other == null) return false;

            if (ReferenceEquals(this, other)) return true;

            return p1.Equals(other.p1) && p2.Equals(other.p2) && p3.Equals(other.p3);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) { return Equals(obj as Face); }

        ///
        public static bool operator ==(Face? left, Face? right) {
            if (left is null && right is null) return true;
            if (left is null) return false;

            return left.Equals(right);
        }

        ///
        public static bool operator !=(Face? left, Face? right) {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            return p1.GetHashCode() ^ p2.GetHashCode() ^ p3.GetHashCode();
        }
    }
}