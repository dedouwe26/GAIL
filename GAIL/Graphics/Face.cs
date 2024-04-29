namespace GAIL.Graphics
{
    /// <summary>
    /// A face (triangle) of a mesh.
    /// </summary>
    /// <param name="p1">Point 1.</param>
    /// <param name="p2">Point 2.</param>
    /// <param name="p3">Point 3.</param>
    public class Face(Vertex p1, Vertex p2, Vertex p3) {
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
    }
}