using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace GAIL.Graphics.Material;

/// <summary>
/// Represents a shader.
/// </summary>
public interface IShader : IDisposable {
    /// <summary>
    /// Which attributes a vertex must contain in order for this shader to work correctly.
    /// </summary>
    public ImmutableArray<FormatInfo> RequiredAttributes { get; }
    /// <summary>
    /// Which uniforms must be set in order for this shader to work correctly.
    /// </summary>
    public ImmutableArray<FormatInfo> RequiredUniforms { get; }
}