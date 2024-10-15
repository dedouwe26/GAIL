namespace GAIL.Graphics.Renderer;

/// <summary>
/// Represents a class that can render stuff.
/// </summary>
public interface IRenderer<TSettings> : IDisposable where TSettings : Settings {
    /// <summary>
    /// Renders the current frame.
    /// </summary>
    public void Render();
    /// <summary>
    /// The settings of the renderer.
    /// </summary>
    public TSettings Settings { get; }
    /// <summary>
    /// Resizes the renderer output.
    /// </summary>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    public void Resize(int width, int height);
}