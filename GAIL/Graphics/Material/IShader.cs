using System.Collections.ObjectModel;

namespace GAIL.Graphics.Material;

/// <summary>
/// Represents a shader.
/// </summary>
public interface IShader : IDisposable {
    /// <summary>
    /// Which attributes a vertex must contain in order for this shader to work correctly.
    /// </summary>
    public ReadOnlyCollection<AttributeType> RequiredAttributes { get; }
    /// <summary>
    /// Which uniforms must be set in order for this shader to work correctly.
    /// </summary>
    public ReadOnlyCollection<AttributeType> RequiredUniforms { get; }
}