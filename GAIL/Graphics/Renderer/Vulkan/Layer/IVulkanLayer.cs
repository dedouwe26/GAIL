using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Renderer.Vulkan.Layer;

/// <summary>
/// Represents a back-end layer specific to Vulkan.
/// </summary>
public interface IVulkanLayer : IBackendLayer {
    /// <summary>
    /// Fills the given command buffer to render.
    /// </summary>
    /// <param name="commands">The given command buffer used to submit render calls.</param>
    public void Render(Commands commands);

    /// <summary>
    /// The vulkan pipeline utility, for custom usage.
    /// </summary>
    public Pipeline Pipeline { get; internal set; }
    /// <summary>
    /// Updates the index of the layer in the renderer.
    /// </summary>
    internal uint Index { set; }
}