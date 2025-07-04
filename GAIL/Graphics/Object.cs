using GAIL.Graphics.Mesh;

namespace GAIL.Graphics;

/// <summary>
/// A renderable object.
/// </summary>
public class Object {
    /// <summary>
    /// The baked vertices to render.
    /// </summary>
    public Mesh.Mesh mesh;
    /// <summary>
    /// The material or style of this object.
    /// </summary>
    public Material.Material material;
    /// <summary>
    /// Creates a new object.
    /// </summary>
    /// <param name="mesh">The mesh or shape of this object.</param>
    /// <param name="material">The material or style of this object.</param>
    public Object(Mesh.Mesh mesh, Material.Material material) {
        this.mesh = mesh;
        this.material = material;
    }
}