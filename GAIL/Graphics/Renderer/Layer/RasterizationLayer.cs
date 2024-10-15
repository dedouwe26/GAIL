namespace GAIL.Graphics.Renderer.Layer;

/// <summary>
/// Represents an abstraction for the back-end rasterization layer.
/// </summary>
public interface IRasterizationLayer : IBackendLayer {
    /// <summary>
    /// The rasterization layer settings.
    /// </summary>
    public IRasterizationLayerSettings Settings { get; }
}
/// <summary>
/// Represents an abstraction for the back-end rasterization layer.
/// </summary>
public abstract class RasterizationLayer<TRenderer> : BackendLayer<TRenderer>, IRasterizationLayer where TRenderer : IRenderer {
    /// <inheritdoc/>
    public abstract IRasterizationLayerSettings Settings { get; }
}