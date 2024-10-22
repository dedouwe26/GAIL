using GAIL.Core;
using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// The settings of the current renderer.
/// </summary>
/// <typeparam name="TBackendLayer">The type of back-end layer the renderer uses.</typeparam>
public interface IRendererSettings<TBackendLayer> where TBackendLayer : IBackendLayer {
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
    /// The layers used by the renderer. Default is empty.
    /// </summary>
    public TBackendLayer[] Layers { get; set; }
}

/// <summary>
/// The values of the settings for a renderer.
/// </summary>
/// <typeparam name="TBackendLayer">The type of back-end layer the renderer uses.</typeparam>
public struct RendererSettings<TBackendLayer> where TBackendLayer : IBackendLayer {
    /// <summary>
    /// If the renderer should render. Defaults to true.
    /// </summary>
    /// <remarks>
    /// Will be turned off if the width or the height of window framebuffer is zero.
    /// </remarks>
    public bool ShouldRender = true;
    /// <summary>
    /// The clear value of the layer. Defaults to a new color where the RGBA values are 0.
    /// </summary>
    public Color ClearValue = new(0, 0, 0, 0);
    /// <summary>
    /// The max frames that can be rendered at once. Defaults to 2.
    /// </summary>
    public uint MaxFramesInFlight = 2;
    /// <summary>
    /// The layers used by the renderer. Default is empty.
    /// </summary>
    public TBackendLayer[] Layers = [];

    /// <summary>
    /// Creates new values of the renderer settings.
    /// </summary>
    public RendererSettings() { }
}

/// <summary>
/// The settings of the current renderer.
/// </summary>
/// <typeparam name="TBackendLayer">The type of back-end layer the renderer uses.</typeparam>
/// <typeparam name="TRenderer">The renderer corresponding to these settings.</typeparam>
public abstract class RendererSettings<TRenderer, TBackendLayer> : IRendererSettings<TBackendLayer> where TRenderer : IRenderer<TBackendLayer> where TBackendLayer : IBackendLayer {
    /// <summary>
    /// The renderer of these settings.
    /// </summary>
    protected readonly TRenderer renderer;

    /// <summary>
    /// Creates new settings for the renderer.
    /// </summary>
    /// <param name="renderer">The renderer to use for these settings.</param>
    /// <param name="values">The initial values of these settings.</param>
    protected RendererSettings(TRenderer renderer, ref RendererSettings<TBackendLayer> values) {
        this.renderer = renderer;
        shouldRender = values.ShouldRender;
        clearValue = values.ClearValue;
        maxFramesInFlight = values.MaxFramesInFlight;
        layers = values.Layers;
    }
    
    /// <inheritdoc/>
    public virtual bool ShouldRender { get => shouldRender; set => shouldRender = value; }
    /// <summary>
    /// If the renderer should render. Defaults to true.
    /// </summary>
    /// <remarks>
    /// Will be turned off if the width or the height of window framebuffer is zero.
    /// </remarks>
    protected bool shouldRender;
    /// <inheritdoc/>
    public virtual Color ClearValue { get => clearValue; set => throw new NotImplementedException(); }
    /// <summary>
    /// The clear value of the layer. Defaults to a new color where the RGBA values are 0.
    /// </summary>
    protected Color clearValue;

    /// <inheritdoc/>
    public virtual uint MaxFramesInFlight { get => maxFramesInFlight; set => throw new NotImplementedException(); }
    /// <summary>
    /// The max frames that can be rendered at once. Defaults to 2.
    /// </summary>
    protected uint maxFramesInFlight;
    
    /// <inheritdoc/>
    public virtual TBackendLayer[] Layers { get => layers; set => throw new NotImplementedException(); }
    /// <summary>
    /// The layers used by the renderer. Default is empty.
    /// </summary>
    protected TBackendLayer[] layers;
}