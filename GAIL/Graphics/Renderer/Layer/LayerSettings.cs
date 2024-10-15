using GAIL.Core;

namespace GAIL.Graphics.Renderer.Layer;

/// <summary>
/// Settings for a rasterization layer.
/// </summary>
public interface IRasterizationLayerSettings {
    /// <summary>
    /// If the renderer should render. Defaults to true.
    /// </summary>
    /// <remarks>
    /// Will be turned off if the width or the height of window framebuffer is zero.
    /// </remarks>
    public bool ShouldRender { get; set; }
    /// <summary>
    /// The clear value of the layer. Defaults to a new color where the RGBA values are 0.
    /// </summary>
    public Color ClearValue { get; set; }
    /// <summary>
    /// The max frames that can be rendered at once. Defaults to 2.
    /// </summary>
    public uint MaxFramesInFlight { get; set; }
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
}

/// <summary>
/// Settings for a rasterization layer.
/// </summary>
public abstract class RasterizationLayerSettings<TLayer> : IRasterizationLayerSettings where TLayer : IRasterizationLayer {
    /// <summary>
    /// The renderer of these settings.
    /// </summary>
    protected readonly TLayer layer;
    /// <summary>
    /// Creates new rasterization layer settings.
    /// </summary>
    /// <param name="layer">The renderer to use for these settings.</param>
    protected RasterizationLayerSettings(TLayer layer) {
        this.layer = layer;
    }
    /// <inheritdoc/>
    public virtual bool ShouldRender { get => shouldRender; set => shouldRender = value; }
    /// <summary>
    /// If the renderer should render. Defaults to true.
    /// </summary>
    /// <remarks>
    /// Will be turned off if the width or the height of window framebuffer is zero.
    /// </remarks>
    protected bool shouldRender = true;
    /// <inheritdoc/>
    public virtual Color ClearValue { get => clearValue; set => throw new NotSupportedException(); }
    /// <summary>
    /// The clear value of the layer. Defaults to a new color where the RGBA values are 0.
    /// </summary>
    protected Color clearValue = new(0, 0, 0, 0);
    /// <inheritdoc/>
    public virtual uint MaxFramesInFlight { get => maxFramesInFlight; set => throw new NotSupportedException(); }
    /// <summary>
    /// The max frames that can be rendered at once. Defaults to 2.
    /// </summary>
    protected uint maxFramesInFlight = 2;
    /// <inheritdoc/>
    public virtual FillMode FillMode { get => fillMode; set => throw new NotSupportedException(); }
    /// <summary>
    /// What part of the triangle to render. Defaults to <see cref="FillMode.Face"/>.
    /// </summary>
    protected FillMode fillMode = FillMode.Face;
    /// <inheritdoc/>
    public virtual FrontFaceMode FrontFaceMode { get => frontFaceMode; set => throw new NotSupportedException(); }
    /// <summary>
    /// How the front face is determined. Defaults to <see cref="FrontFaceMode.Clockwise"/>.
    /// </summary>
    protected FrontFaceMode frontFaceMode = FrontFaceMode.Clockwise;
    /// <inheritdoc/>
    public virtual CullMode CullMode { get => cullMode; set => throw new NotSupportedException(); }
    /// <summary>
    /// The type of face culling. Defaults to <see cref="CullMode.BackFace"/>.
    /// </summary>
    protected CullMode cullMode = CullMode.BackFace;
}

// TODO: Raytracing settings