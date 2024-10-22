using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Layer;

/// <summary>
/// A front-end layer that is used for rendering.
/// </summary>
public interface ILayer<TBackend> : IDisposable where TBackend : IBackendLayer  {
    /// <summary>
    /// The back-end layer that this uses.
    /// </summary>
    public TBackend BackendLayer { get; }
    
    /// <summary>
    /// Initializes this front-end layer.
    /// </summary>
    /// <param name="backendLayer">The initalized back-end layers.</param>
    public void Initialize(TBackend backendLayer);
    /// <summary>
    /// Renders this front-end layer.
    /// </summary>
    public void Render();
}
