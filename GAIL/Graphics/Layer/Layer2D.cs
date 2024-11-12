using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Layer;

/// <summary>
/// A layer for 2D graphics.
/// </summary>
public class Layer2D : ILayer<IRasterizationLayer> {
    /// <summary>
    /// If this 2D layer is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    IRasterizationLayer? ILayer<IRasterizationLayer>.BackendLayer => backendLayer;

    private IRasterizationLayer? backendLayer;
    /// <summary>
    /// Creates a new 2D layer.
    /// </summary>
    public Layer2D() {
        IsDisposed = false;
    }
    /// <inheritdoc/>
    public void Initialize(IRasterizationLayer backendLayer) {
        this.backendLayer = backendLayer;
    }

    public void Render(Object obj) {

    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) return;

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}