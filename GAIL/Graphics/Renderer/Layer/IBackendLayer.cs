namespace GAIL.Graphics.Renderer.Layer;

/// <summary>
/// Represents an abstraction for a back-end layer.
/// </summary>
public interface IBackendLayer : IDisposable {
    // /// <summary>
    // /// Renders the back-end layer.
    // /// </summary>
    // /// <param name="currentFrame">The current frame in flight.</param>
    // public void Render(uint currentFrame);
    /// <summary>
    /// Whether the renderer should re-record the command buffer.
    /// </summary>
    public bool ShouldRecord { get; }
}