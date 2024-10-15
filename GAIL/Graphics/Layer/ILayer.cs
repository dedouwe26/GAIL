using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Layer;

/// <summary>
/// A front-end layer that is used for rendering.
/// </summary>
public interface ILayer<TBackend> : IDisposable where TBackend : IBackendLayer  {
    /// <summary>
    /// Initializes this front-end layer.
    /// </summary>
    /// <param name="backendLayer">The initalized back-end layers.</param>
    public void Initialize(IBackendLayer backendLayer);
    /// <summary>
    /// Renders this front-end layer.
    /// </summary>
    /// <param name="backendLayer"></param>
    public void Render(IBackendLayer backendLayer);
}

/// <summary>
/// A front-end layer that is used for rendering.
/// </summary>
public abstract class Layer<TBackend> : ILayer<TBackend> where TBackend : IBackendLayer {
    /// <inheritdoc/>
    public abstract void Dispose();

    /// <summary>
    /// Initializes this front-end layer.
    /// </summary>
    /// <param name="backendLayer">The initalized back-end layers.</param>
    public abstract void Initialize(TBackend backendLayer);
    /// <inheritdoc/>
    public void Initialize(IBackendLayer backendLayer) {
        
    }

    /// <summary>
    /// Renders this front-end layer.
    /// </summary>
    /// <param name="backendLayer"></param>
    public abstract void Render(TBackend backendLayer);

    /// <inheritdoc/>
    public void Render(IBackendLayer backendLayer)
    {
        throw new NotImplementedException();
    }
}
