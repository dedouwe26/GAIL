namespace GAIL.Graphics
{
    /// <summary>
    /// A face (triangle) of a mesh.
    /// </summary>
    public class Face {
        public Vertex p1;
        public Vertex p2;
        public Vertex p3;
        public Face(Vertex p1, Vertex p2, Vertex p3) {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }
}