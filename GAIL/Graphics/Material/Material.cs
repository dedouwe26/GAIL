using System.Collections.ObjectModel;

namespace GAIL.Graphics.Material;

/// <summary>
/// Contains information about how anything is drawn to the window, contains the shader and shader data.
/// </summary>
public interface IMaterial {
    /// <summary>
    /// Applies all the uniforms.
    /// </summary>
    /// <param name="shader">The shader of which to set the uniforms.</param>
    public abstract void Apply(IShader shader);
}

/// <summary>
/// Contains information about how anything is drawn to the window, contains the shader and shader data.
/// </summary>
public abstract class Material : IMaterial {

    /// <summary>
    /// Creates a new material.
    /// </summary>
    protected Material() { }

    // protected bool SetUniform() {

    // }
    
    /// <inheritdoc/>
    public abstract void Apply(IShader shader);
}