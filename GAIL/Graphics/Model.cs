using GAIL.Graphics.Material;

namespace GAIL.Graphics
{
    /// <summary>
    /// A model in 3D / 2D space based on a mesh.
    /// This can be rendered.
    /// </summary>
    public class Model {
        /// <summary>
        /// The mesh (3D data) of this Model. 
        /// </summary>
        public Mesh.Mesh mesh;
        /// <summary>
        /// The material (3D render data) of this Model.
        /// </summary>
        public IMaterial material;
        /// <summary>
        /// Creates a model.
        /// </summary>
        /// <param name="mesh">The mesh to use.</param>
        /// <param name="material">The material to use.</param>
        public Model(Mesh.Mesh mesh, IMaterial material) {
            this.mesh = mesh;
            this.material = material;
        }
    }
}