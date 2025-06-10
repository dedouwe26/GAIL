namespace GAIL.Graphics.Renderer.Layer;

/// <summary>
/// Represents an abstraction for the back-end rasterization layer.
/// </summary>
public interface IRasterizationLayer : IBackendLayer {
    /// <summary>
    /// The rasterization layer settings.
    /// </summary>
    public IRasterizationLayerSettings Settings { get; }
    /// <summary>
    /// Renders an object.
    /// </summary>
    /// <param name="obj">The object to render.</param>
    /// <returns>Whether it succeeded.</returns>
    public bool Render(Object obj);
}