using GAIL.Core;
using GAIL.Graphics.Renderer.Vulkan;
using OxDED.Terminal.Logging;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// Represents a renderer that uses the Vulkan Graphics API.
/// </summary>
public class VulkanRenderer : IRenderer {
    internal Logger Logger;
    /// <summary>
    /// Creates a new Vulkan Renderer.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    public VulkanRenderer(Logger logger) {
        Logger = logger;
    }
    /// <summary>
    /// The vulkan devices utility, for custom usage.
    /// </summary>
    public Device? device;
    /// <summary>
    /// The vulkan window surface utility, for custom usage.
    /// </summary>
    public Surface? surface;

    /// <summary>
    /// The vulkan swapchain utility, for custom usage.
    /// </summary>
    public SwapChain? swapchain;
    /// <summary>
    /// The vulkan instance utility, for custom usage.
    /// </summary>
    public Instance? instance;

    /// <inheritdoc/>
    public void Initialize(Application.Globals globals, AppInfo appInfo) {
        if (!API.Glfw.VulkanSupported()) {
            Logger.LogFatal("Vulkan: Not Supported!");
            throw new APIBackendException("Vulkan", "Not Supported");
        }

        instance = new Instance(this, appInfo);
        surface = new Surface(this, globals.windowManager);
        device = new Device(this, ref surface);
        swapchain = new SwapChain(this, globals.windowManager);
    }

    /// <inheritdoc/>
    public void Dispose() {
        Logger.LogDebug("Terminating Vulkan.");
        unsafe {
            swapchain!.Dispose();
            device!.Dispose();
            surface!.Dispose();
            instance!.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}