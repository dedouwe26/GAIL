using GAIL.Graphics.Renderer.Layer;
using OxDED.Terminal.Logging;

namespace GAIL.Graphics.Renderer.Vulkan.Layer;

/// <summary>
/// Vulkan implementation of the rasterization layer settings.
/// </summary>
public class VulkanRasterizationLayerSettings : RasterizationLayerSettings<VulkanRasterizationLayer> {
    /// <summary>
    /// Creates new a new Vulkan implementation of the rasterization layer settings.
    /// </summary>
    /// <param name="layer">The vulkan implementation of the rasterization layer.</param>
    /// <param name="values">The initial values of these settings.</param>
    public VulkanRasterizationLayerSettings(VulkanRasterizationLayer layer, ref RasterizationLayerSettings values) : base(layer, ref values) { }
    
    /// <inheritdoc/>
    public override CullMode CullMode { get => base.CullMode; set {
        layer.Pipeline.Dispose();
        cullMode = value;
        layer.Pipeline = new Pipeline(layer);
    } }
    /// <inheritdoc/>
    public override FillMode FillMode { get => base.FillMode; set {
        layer.Pipeline.Dispose();
        fillMode = value;
        layer.Pipeline = new Pipeline(layer);
    } }
    /// <inheritdoc/>
    public override FrontFaceMode FrontFaceMode { get => base.FrontFaceMode; set {
        layer.Pipeline.Dispose();
        frontFaceMode = value;
        layer.Pipeline = new Pipeline(layer);
    } }
    /// <inheritdoc/>
    public override Shader Shader { get => base.Shader; set {
        layer.Pipeline.Dispose();
        shader = value;
        layer.Pipeline = new Pipeline(layer);
    } }
}

/// <summary>
/// The Vulkan implementation of the back-end rasterization layer.
/// </summary>
public class VulkanRasterizationLayer : IVulkanLayer, IRasterizationLayer {
    internal VulkanRasterizationLayer(VulkanRenderer renderer, uint index, ref RasterizationLayerSettings settings) {
        Logger = renderer.Logger;
        Renderer = renderer;
        Index = index;
        this.settings = new(this, ref settings);

        Logger.LogDebug("Creating a Vulkan rasterization back-end layer.");

        Pipeline = new Pipeline(this);
    }
    /// <summary>
    /// If the Vulkan rasterization layer is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public Pipeline Pipeline { get; internal set; }

    /// <summary>
    /// The renderer corresponding to this back-end layer.
    /// </summary>
    public readonly VulkanRenderer Renderer;
    /// <summary>
    /// The logger of this back-end layer.
    /// </summary>
    public readonly Logger Logger;
    private readonly VulkanRasterizationLayerSettings settings;
    /// <inheritdoc/>
    public IRasterizationLayerSettings Settings => settings;
    internal uint Index;
    Pipeline IVulkanLayer.Pipeline { get => Pipeline; set => Pipeline = value; }
    uint IVulkanLayer.Index { set {
        Pipeline.Dispose();
        Index = value;
        Pipeline = new(this);
    } }

    /// <inheritdoc/>
    public void Render(Commands commands) {
        if (!settings.ShouldRender) return;

        commands.BindPipeline(Renderer, Pipeline);

        // TODO: Temporary
        commands.Draw(Renderer, vertexCount:3);
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) return;
        
        Pipeline.Dispose();

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}