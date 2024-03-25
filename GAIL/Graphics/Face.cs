namespace GAIL.Graphics
{
    /// <summary>
    /// A face (triangle) of a mesh.
    /// </summary>
    /// <param name="p1">Point 1.</param>
    /// <param name="p2">Point 2.</param>
    /// <param name="p3">Point 3.</param>
    public class Face(Vertex p1, Vertex p2, Vertex p3) {
        public Vertex p1 = p1;
        public Vertex p2 = p2;
        public Vertex p3 = p3;
    }
}