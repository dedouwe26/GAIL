using GAIL.Core;
using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// The settings of the current renderer.
/// </summary>
public interface IRendererSettings<TLayer> where TLayer : I {
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
    public TLayer[] LayerSettings { get; set; }
}

/// <summary>
/// The values of the settings for a renderer.
/// </summary>
public class RendererSettings<TLayer> : IRendererSettings<TLayer> where TLayer : IBackendLayer {
    /// <inheritdoc/>
    public bool ShouldRender { get; set; } = true;
    /// <inheritdoc/>
    public Color ClearValue { get; set; } = new(0, 0, 0, 0);
    /// <inheritdoc/>
    public uint MaxFramesInFlight { get; set; } = 2;
    /// <inheritdoc/>
    public TLayer[] Layers { get; set; } = [];}