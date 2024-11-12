using System.Collections.ObjectModel;

namespace GAIL.Graphics.Material;

/// <summary>
/// Contains information about how anything is drawn to the window, contains the shader and shader data.
/// </summary>
public interface IMaterial {
    /// <summary>
    /// Gives the shader.
    /// </summary>
    /// <returns>The shader to use.</returns>
    public IShader GetShader();
    /// <summary>
    /// Which attributes a vertex must contain in order for this shader to work correctly.
    /// </summary>
    public ReadOnlyCollection<AttributeType> RequiredAttributes { get; }
    /// <summary>
    /// Which uniforms must be set in order for this shader to work correctly.
    /// </summary>
    public ReadOnlyCollection<AttributeType> RequiredUniforms { get; }
}

/// <summary>
/// Contains information about how anything is drawn to the window, contains the shader and shader data.
/// </summary>
public abstract class Material : IMaterial {
    /// <summary>
    /// The shader used by this material.
    /// </summary>
    protected IShader shader;

    /// <summary>
    /// Creates a new material.
    /// </summary>
    /// <param name="shader">The shader of this material.</param>
    public Material(IShader shader) {
        this.shader = shader;
    }

    /// <inheritdoc/>
    public virtual ReadOnlyCollection<AttributeType> RequiredAttributes => shader.RequiredAttributes;

    /// <inheritdoc/>
    public virtual ReadOnlyCollection<AttributeType> RequiredUniforms => shader.RequiredUniforms;

    /// <inheritdoc/>
    public virtual IShader GetShader() {
        return shader;
    }
}