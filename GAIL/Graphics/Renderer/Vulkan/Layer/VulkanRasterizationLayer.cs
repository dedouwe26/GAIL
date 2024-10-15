using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Renderer.Vulkan.Layer;

/// <summary>
/// Vulkan implementation of the rasterization layer settings.
/// </summary>
public class VulkanRasterizationLayerSettings : RasterizationLayerSettings<VulkanRasterizationLayer> {
    /// <summary>
    /// Creates new a new Vulkan implementation of the rasterization layer settings.
    /// </summary>
    /// <param name="layer">The vulkan implementation of the rasterization layer.</param>
    public VulkanRasterizationLayerSettings(VulkanRasterizationLayer layer) : base(layer) { }
    
}

/// <summary>
/// The Vulkan implementation of the back-end rasterization layer.
/// </summary>
public class VulkanRasterizationLayer : RasterizationLayer<VulkanRenderer> {
    /// <summary>
    /// Creates a new Vulkan implementation of the back-end rasterization layer.
    /// </summary>
    public VulkanRasterizationLayer(VulkanRenderer renderer) {
        settings = new(this);
    }
    /// <summary>
    /// If the Vulkan rasterization layer is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }
    private readonly VulkanRasterizationLayerSettings settings;
    /// <inheritdoc/>
    public override IRasterizationLayerSettings Settings => settings;

    /// <inheritdoc/>
    public override void Dispose() {
        if (IsDisposed) return;

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }

    // /// <inheritdoc/>
    // public override void Initialize(VulkanRenderer renderer) {

    // }

    /// <inheritdoc/>
    public override void Render() {

    }
}