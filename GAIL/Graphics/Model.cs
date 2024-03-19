namespace GAIL.Graphics
{
    /// <summary>
    /// A model in 3D / 2D space based on a mesh.
    /// This can be rendered.
    /// </summary>
    public class Model {
        public Mesh mesh;
        public IMaterial material;
        /// <summary>
        /// Creates a model.
        /// </summary>
        /// <param name="mesh">The mesh to use.</param>
        /// <param name="material">The material to use.</param>
        public Model(Mesh mesh, IMaterial material) {
            this.mesh = mesh;
            this.material = material;
        }
    }
}