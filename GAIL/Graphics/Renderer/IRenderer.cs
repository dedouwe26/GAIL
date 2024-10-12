namespace GAIL.Graphics.Renderer;

/// <summary>
/// The settings of the current renderer.
/// </summary>
public struct Settings {
    /// <summary>
    /// The max frames that can be rendered at once.
    /// </summary>
    public uint MaxFramesInFlight;
}

/// <summary>
/// Represents a class that can render stuff.
/// </summary>
public interface IRenderer : IDisposable {
    /// <summary>
    /// Renders the current frame.
    /// </summary>
    public void Render();
    /// <summary>
    /// Updates the configuration of the current renderer.
    /// </summary>
    public void UpdateSettings(Settings newSettings);
    /// <summary>
    /// Resizes the renderer output.
    /// </summary>
    public void Resize();
}