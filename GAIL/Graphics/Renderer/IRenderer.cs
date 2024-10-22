using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// Represents a class that can render stuff.
/// </summary>
public interface IRenderer<TBackendLayer> : IDisposable where TBackendLayer : IBackendLayer {
    /// <summary>
    /// Renders the current frame.
    /// </summary>
    public void Render();
    /// <summary>
    /// The settings of the renderer.
    /// </summary>
    public IRendererSettings<TBackendLayer> Settings { get; }
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
    /// True, if it could create a back-end layer.
    /// </returns>
    /// <param name="backendLayer">The created back-end rasterization layer. It is null if the return value is false.</param>
    /// <param name="settings">The initial settings values of the back-end rasterization layer settings</param>
    public bool CreateRasterizationLayer(out IRasterizationLayer? backendLayer, ref RasterizationLayerSettings settings);
    /// <summary>
    /// The back-end layers for rendering.
    /// </summary>
    public IEnumerable<IBackendLayer> BackendLayers { get; }
}