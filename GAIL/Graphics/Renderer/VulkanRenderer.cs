using GAIL.Core;
using GAIL.Graphics.Renderer.Vulkan;
using OxDED.Terminal.Logging;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// Represents a renderer that uses the Vulkan Graphics API.
/// </summary>
public class VulkanRenderer : IRenderer {
    /// <summary>
    /// If this class is already disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }
    internal Logger Logger;
    
    /// <summary>
    /// The vulkan devices utility, for custom usage.
    /// </summary>
    public Device device;
    /// <summary>
    /// The vulkan window surface utility, for custom usage.
    /// </summary>
    public Surface surface;

    /// <summary>
    /// The vulkan swapchain utility, for custom usage.
    /// </summary>
    public SwapChain swapchain;
    /// <summary>
    /// The vulkan renderpass utility, for custom usage.
    /// </summary>
    public RenderPass renderPass;
    /// <summary>
    /// The vulkan instance utility, for custom usage.
    /// </summary>
    public Instance instance;

    /// <summary>
    /// Creates a new Vulkan Renderer.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="globals">The globals of the application.</param>
    /// <param name="appInfo">The application info.</param>
    public VulkanRenderer(Logger logger, Application.Globals globals, AppInfo appInfo) {
        Logger = logger;
        if (!API.Glfw.VulkanSupported()) {
            Logger.LogFatal("Vulkan: Not Supported!");
            throw new APIBackendException("Vulkan", "Not Supported");
        }

        instance = new Instance(this, appInfo);
        surface = new Surface(this, globals.windowManager);
        device = new Device(this);
        swapchain = new SwapChain(this, globals.windowManager);

        renderPass = new RenderPass(this);
        // Shaders
        // Pipeline
        swapchain.CreateFramebuffers(renderPass);
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) { return; }
        
        Logger.LogDebug("Terminating Vulkan.");
        // ~Pipeline
        renderPass.Dispose();
        swapchain.Dispose();
        device.Dispose();
        surface.Dispose();
        instance.Dispose();
        
        GC.SuppressFinalize(this);
    }
}