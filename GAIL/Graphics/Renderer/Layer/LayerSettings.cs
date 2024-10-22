using GAIL.Graphics.Renderer.Vulkan;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Layer;

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
/// Settings for a rasterization layer.
/// </summary>
public interface IRasterizationLayerSettings {
    /// <summary>
    /// If the renderer should render. Defaults to true.
    /// </summary>
    public bool ShouldRender { get; set; }
    /// <summary>
    /// What part of the triangle to render. Defaults to <see cref="FillMode.Face"/>.
    /// </summary>
    public FillMode FillMode { get; set; }
    /// <summary>
    /// How the front face is determined. Defaults to <see cref="FrontFaceMode.Clockwise"/>.
    /// </summary>
    public FrontFaceMode FrontFaceMode { get; set; }
    /// <summary>
    /// The type of face culling. Defaults to <see cref="CullMode.BackFace"/>.
    /// </summary>
    public CullMode CullMode { get; set; }
    /// <summary>
    /// The shader stages to use for this layer.
    /// </summary>
    public Shaders Shaders { get; set; }

}
/// <summary>
/// The values of the settings for a rasterization layer.
/// </summary>
public struct RasterizationLayerSettings {
    /// <summary>
    /// If the renderer should render. Defaults to true.
    /// </summary>
    public bool ShouldRender = true;
    /// <summary>
    /// What part of the triangle to render. Defaults to <see cref="FillMode.Face"/>.
    /// </summary>
    public FillMode FillMode = FillMode.Face;
    /// <summary>
    /// How the front face is determined. Defaults to <see cref="FrontFaceMode.Clockwise"/>.
    /// </summary>
    public FrontFaceMode FrontFaceMode = FrontFaceMode.Clockwise;
    /// <summary>
    /// The type of face culling. Defaults to <see cref="CullMode.BackFace"/>.
    /// </summary>
    public CullMode CullMode = CullMode.BackFace;
    /// <summary>
    /// The shader stages to use for this layer.
    /// </summary>
    public required Shaders Shaders;

    /// <summary>
    /// Creates new values of the rasterization layer settings.
    /// </summary>
    public RasterizationLayerSettings() { }
}
/// <summary>
/// Settings for a rasterization layer.
/// </summary>
public abstract class RasterizationLayerSettings<TLayer> : IRasterizationLayerSettings where TLayer : IRasterizationLayer {
    /// <summary>
    /// The layer of these settings.
    /// </summary>
    protected readonly TLayer layer;

    /// <summary>
    /// Creates new settings for the back-end rasterization layer.
    /// </summary>
    /// <param name="layer">The back-end rasterization layer to use for these settings.</param>
    /// <param name="values">The initial values of these settings.</param>
    protected RasterizationLayerSettings(TLayer layer, ref RasterizationLayerSettings values) {
        this.layer = layer;
        shouldRender = values.ShouldRender;
        fillMode = values.FillMode;
        cullMode = values.CullMode;
        shaders = values.Shaders;
    }

    /// <inheritdoc/>
    public virtual bool ShouldRender { get => shouldRender; set => shouldRender = value; }
    /// <summary>
    /// If the renderer should render. Defaults to true.
    /// </summary>
    protected bool shouldRender;

    /// <inheritdoc/>
    public virtual FillMode FillMode { get => fillMode; set => throw new NotImplementedException(); }
    /// <summary>
    /// What part of the triangle to render. Defaults to <see cref="FillMode.Face"/>.
    /// </summary>
    protected FillMode fillMode;
    /// <inheritdoc/>
    public virtual FrontFaceMode FrontFaceMode { get => frontFaceMode; set => throw new NotImplementedException(); }
    /// <summary>
    /// How the front face is determined. Defaults to <see cref="FrontFaceMode.Clockwise"/>.
    /// </summary>
    protected FrontFaceMode frontFaceMode = FrontFaceMode.Clockwise;
    /// <inheritdoc/>
    public virtual CullMode CullMode { get => cullMode; set => throw new NotImplementedException(); }
    /// <summary>
    /// The type of face culling. Defaults to <see cref="CullMode.BackFace"/>.
    /// </summary>
    protected CullMode cullMode;
    /// <inheritdoc/>
    public virtual Shaders Shaders { get => shaders; set => throw new NotImplementedException(); }
    /// <summary>
    /// The shader stages to use for this layer.
    /// </summary>
    protected Shaders shaders;
}

// TODO: Raytracing settings