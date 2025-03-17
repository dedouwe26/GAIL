using System.Collections.ObjectModel;
using GAIL.Core;
using OxDED.Terminal.Assertion;

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
    // private static IShader CreateShader(GraphicsManager manager, byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null) {
    //     IShader shader = Assert.IsNotNull(manager.CreateShader(vertexShader, fragmentShader, geometryShader)).OnFailure((Assertion _) => {
    //         manager.Logger.LogError("Failed to create shader.");
    //         throw new InvalidOperationException("Failed to create shader.");
    //     }).As<ReferenceAssertion<IShader?>>()!.Asserter().value!;
    //     return shader;
    // }

    /// <summary>
    /// Creates a new material.
    /// </summary>
    /// <param name="manager">The graphics manager used to create the shaders.</param>
    /// <param name="vertexShader">The per-vertex shader (in SPIR-V compiled).</param>
    /// <param name="fragmentShader">The per-pixel shader (in SPIR-V compiled).</param>
    /// <param name="geometryShader">The geometry shader (in SPIR-V compiled).</param>
    protected Material(GraphicsManager manager, byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null) {

    }

    /// <inheritdoc/>
    public virtual ReadOnlyCollection<AttributeType> RequiredUniforms => RequiredUniforms;
}