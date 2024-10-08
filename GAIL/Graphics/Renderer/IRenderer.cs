namespace GAIL.Graphics.Renderer;

/// <summary>
/// Represents a class that can render stuff.
/// </summary>
public interface IRenderer : IDisposable {
    /// <summary>
    /// Renders the current frame.
    /// </summary>
    public void Render();
}