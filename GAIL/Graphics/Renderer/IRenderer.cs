namespace GAIL.Graphics.Renderer;

/// <summary>
/// Represents a class that can render stuff.
/// </summary>
public interface IRenderer : IDisposable {
    /// <summary>
    /// Initializes the renderer.
    /// </summary>
    public void Initialize(Application.Globals globals, AppInfo appInfo);
}