using System.Collections.ObjectModel;
using GAIL.Core;

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
    private static IShader CreateShader(GraphicsManager manager, byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null) {
        IShader shader = Assert.NotNull(manager.Logger, manager.CreateShader(vertexShader, fragmentShader, geometryShader), "Failed to create shader.") ? throw new InvalidOperationException();
        return  
    }
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
    /// <summary>
    /// Creates a new material.
    /// </summary>
    /// <param name="manager">The graphics manager used to create the shaders.</param>
    /// <param name="vertexShader">The per-vertex shader (in SPIR-V compiled).</param>
    /// <param name="fragmentShader">The per-pixel shader (in SPIR-V compiled).</param>
    /// <param name="geometryShader">The geometry shader (in SPIR-V compiled).</param>
    protected Material(GraphicsManager manager, byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null) : this(
        
    ) { }

    /// <inheritdoc/>
    public virtual ReadOnlyCollection<AttributeType> RequiredAttributes => shader.RequiredAttributes;

    /// <inheritdoc/>
    public virtual ReadOnlyCollection<AttributeType> RequiredUniforms => shader.RequiredUniforms;

    /// <inheritdoc/>
    public virtual IShader GetShader() {
        return shader;
    }
}