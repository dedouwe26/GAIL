using System.Collections.ObjectModel;

namespace GAIL.Graphics.Material;

/// <summary>
/// Contains information about how anything is drawn to the window, contains the shader and shader data.
/// </summary>
public interface IMaterial {
    /// <summary>
    /// Which uniforms must be set in order for this shader to work correctly.
    /// </summary>
    public ReadOnlyCollection<AttributeType> RequiredUniforms { get; }
}

/// <summary>
/// Contains information about how anything is drawn to the window, contains the shader and shader data.
/// </summary>
public abstract class Material : IMaterial {
    // private static IShader CreateShader(GraphicsManager manager, ) {
    //     
    // }

    public readonly IShader Shader;
    
    /// <inheritdoc/>
    public virtual ReadOnlyCollection<AttributeType> RequiredUniforms => Shader.RequiredUniforms;

    /// <summary>
    /// Creates a new material.
    /// </summary>
    /// <param name="shader">The shader corresponding to this material.</param> 
    protected Material(IShader shader) {
        Shader = shader;
    }

    protected bool SetUniform() {

    }
}