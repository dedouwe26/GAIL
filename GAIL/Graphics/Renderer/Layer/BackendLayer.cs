namespace GAIL.Graphics.Renderer.Layer;

/// <summary>
/// Represents an abstraction for a back-end layer.
/// </summary>
public interface IBackendLayer : IDisposable {
    // /// <summary>
    // /// Initializes the back-end layer.
    // /// </summary>
    // /// <param name="renderer">The corresponding renderer.</param>
    // public void Initialize(IRenderer renderer);
    /// <summary>
    /// Renders the back-end layer.
    /// </summary>
    public void Render();
}
/// <summary>
/// Represents an abstraction for a back-end layer.
/// </summary>
public abstract class BackendLayer<TRenderer> : IBackendLayer where TRenderer : IRenderer {
    /// <inheritdoc/>
    public abstract void Dispose();
    // /// <summary>
    // /// Initializes the back-end layer.
    // /// </summary>
    // /// <param name="renderer">The corresponding renderer.</param>
    // /// <exception cref="NotSupportedException">Tried to give the wrong renderer to a layer.</exception>
    // public void Initialize(IRenderer renderer) {
    //     if (renderer is not TRenderer) {
    //         throw new NotSupportedException("Tried to give the wrong renderer to a layer.");
    //     }
    //     Initialize((TRenderer)renderer);
    // }
    // /// <summary>
    // /// Initializes the back-end layer.
    // /// </summary>
    // /// <param name="renderer">The corresponding renderer.</param>
    // public abstract void Initialize(TRenderer renderer);
    /// <summary>
    /// Renders the back-end layer.
    /// </summary>
    public abstract void Render();
}