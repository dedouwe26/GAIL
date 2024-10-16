using GAIL.Core;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// Specifies what part to render.
/// </summary>
public enum FillMode {
    /// <summary>
    /// Only renders the triangle's surface.
    /// </summary>
    Face = PolygonMode.Fill,
    /// <summary>
    /// Only renders the edges of the 
    /// </summary>
    Wireframe = PolygonMode.Line,
    /// <summary>
    /// Only renders the vertices of the triangles to the screen.
    /// </summary>
    Points = PolygonMode.Point
}
/// <summary>
/// Specifies the type of face culling.
/// </summary>
public enum CullMode {
    /// <summary>
    /// Culls the back face (won't render the back face).
    /// </summary>
    /// <remarks>
    /// most applications use backface culling.
    /// </remarks>
    BackFace = CullModeFlags.BackBit,
    /// <summary>
    /// Culls the front face (won't render the front face).
    /// </summary>
    FrontFace = CullModeFlags.FrontBit,
    /// <summary>
    /// Culls the front and the back face (won't render both faces).
    /// </summary>
    Both = CullModeFlags.FrontAndBack,
    /// <summary>
    /// Disables face culling.
    /// </summary>
    None = CullModeFlags.None,
}
/// <summary>
/// How the front face is determined.
/// </summary>
public enum FrontFaceMode {
    /// <summary>
    /// If the three vertices of a triangle are clockwise, then they are facing the camera. 
    /// </summary>
    Clockwise = FrontFace.Clockwise,
    /// <summary>
    /// If the three vertices of a triangle are counter clockwise, then they are facing the camera. 
    /// </summary>
    CounterClockwise = FrontFace.CounterClockwise
}

/// <summary>
/// The settings of the current renderer.
/// </summary>
public abstract class Settings {
    /// <summary>
    /// If the renderer should render. Defaults to true.
    /// </summary>
    /// <remarks>
    /// Will be turned off if the width or the height of window framebuffer is zero.
    /// </remarks>
    public virtual bool ShouldRender { get => shouldRender; set => shouldRender = value; }
    /// <summary>
    /// If the renderer should render. Defaults to true.
    /// </summary>
    /// <remarks>
    /// Will be turned off if the width or the height of window framebuffer is zero.
    /// </remarks>
    protected bool shouldRender = true;
}