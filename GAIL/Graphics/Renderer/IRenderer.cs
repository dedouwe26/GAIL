using GAIL.Graphics.Material;
using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// Represents a class that can render stuff.
/// </summary>
public interface IRenderer<TBackendLayer> : IDisposable where TBackendLayer : IBackendLayer {
    /// <summary>
    /// The settings of the renderer.
    /// </summary>
    public IRendererSettings<TBackendLayer> Settings { get; }

    /// <summary>
    /// Renders the current frame.
    /// </summary>
    public void Render();
    /// <summary>
    /// Resizes the renderer output.
    /// </summary>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    public void Resize(int width, int height);

    /// <summary>
    /// Creates a back-end rasterization layer for rendering.
    /// </summary>
    /// <returns>
    /// The back-end rasterization layer, if it could create a back-end layer.
    /// </returns>
    /// <param name="settings">The initial settings values of the back-end rasterization layer settings.</param>
    public IRasterizationLayer? CreateRasterizationLayer(RasterizationLayerSettings settings);
    /// <summary>
    /// Creates a shader from the following stages.
    /// </summary>
    /// <param name="vertexShader">The per-vertex shader (in SPIR-V compiled).</param>
    /// <param name="fragmentShader">The per-pixel shader (in SPIR-V compiled).</param>
    /// <param name="geometryShader">The geometry shader (in SPIR-V compiled).</param>
    /// <returns>The shader, if it could create a shader.</returns>
    public IShader? CreateShader(FormatInfo[] requiredAttributes, FormatInfo[] requiredUniforms, byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null);
}