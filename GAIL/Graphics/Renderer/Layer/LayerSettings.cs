using GAIL.Graphics.Material;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Layer;

#region Enumerations

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
/// All the levels of MSAA (multi sampling anti-aliasing).
/// </summary>
public enum MSAA {
    /// <summary>
    /// No MSAA (off).
    /// </summary>
    MSAAx1 = SampleCountFlags.Count1Bit,
    /// <summary>
    /// MSAA set to 2.
    /// </summary>
    MSAAx2 = SampleCountFlags.Count2Bit,
    /// <summary>
    /// MSAA set to 4.
    /// </summary>
    MSAAx4 = SampleCountFlags.Count4Bit,
    /// <summary>
    /// MSAA set to 8.
    /// </summary>
    MSAAx8 = SampleCountFlags.Count8Bit,
    /// <summary>
    /// MSAA set to 16.
    /// </summary>
    MSAAx16 = SampleCountFlags.Count16Bit,
    /// <summary>
    /// MSAA set to 32.
    /// </summary>
    MSAAx32 = SampleCountFlags.Count32Bit,
    /// <summary>
    /// MSAA set to 64.
    /// </summary>
    MSAAx64 = SampleCountFlags.Count64Bit,
}

#endregion Enumerations

#region Rasterization Settings

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
    public IShader Shader { get; set; }
    /// <summary>
    /// The shader stages to use for this layer.
    /// </summary>
    public List<Object> RenderList { get; set; }

}
/// <summary>
/// The values of the settings for a rasterization layer.
/// </summary>
public class RasterizationLayerSettings {
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
    public required IShader Shaders;
    /// <summary>
    /// The shader stages to use for this layer.
    /// </summary>
    public required List<Object> RenderList;

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

	protected RasterizationLayerSettings values;
	/// <summary>
	/// Creates new settings for the back-end rasterization layer.
	/// </summary>
	/// <param name="layer">The back-end rasterization layer to use for these settings.</param>
	/// <param name="values">The initial values of these settings.</param>
	protected RasterizationLayerSettings(TLayer layer, RasterizationLayerSettings values) {
        this.layer = layer;
		this.values = values;
	}

    /// <inheritdoc/>
    public virtual bool ShouldRender { get => values.ShouldRender; set => values.ShouldRender = value; }

    /// <inheritdoc/>
    public virtual FillMode FillMode { get => values.FillMode; set => throw new NotImplementedException(); }
    /// <inheritdoc/>
    public virtual FrontFaceMode FrontFaceMode { get => values.FrontFaceMode; set => throw new NotImplementedException(); }
    /// <inheritdoc/>
    public virtual CullMode CullMode { get => values.CullMode; set => throw new NotImplementedException(); }
    /// <inheritdoc/>
    public virtual IShader Shader { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    /// <inheritdoc/>
    public virtual List<Object> RenderList { get => values.RenderList; set => throw new NotImplementedException(); }
}

#endregion Rasterization Settings

// TODO: Raytracing settings