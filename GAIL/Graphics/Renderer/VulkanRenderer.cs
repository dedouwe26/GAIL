using GAIL.Core;
using GAIL.Graphics.Renderer.Vulkan;
using OxDED.Terminal.Logging;
using Silk.NET.Vulkan;
using Device = GAIL.Graphics.Renderer.Vulkan.Device;
using Instance = GAIL.Graphics.Renderer.Vulkan.Instance;
using Pipeline = GAIL.Graphics.Renderer.Vulkan.Pipeline;
using RenderPass = GAIL.Graphics.Renderer.Vulkan.RenderPass;

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
    public readonly Device device;
    /// <summary>
    /// The vulkan window surface utility, for custom usage.
    /// </summary>
    public readonly Surface surface;

    /// <summary>
    /// The vulkan swapchain utility, for custom usage.
    /// </summary>
    public readonly SwapChain swapchain;
    /// <summary>
    /// The vulkan renderpass utility, for custom usage.
    /// </summary>
    public readonly RenderPass renderPass;
    /// <summary>
    /// The vulkan pipeline utility, for custom usage.
    /// </summary>
    public readonly Pipeline pipeline;
    /// <summary>
    /// The vulkan syncronization utility, for custom usage.
    /// </summary>
    public readonly Syncronization syncronization;
    /// <summary>
    /// The vulkan commandbuffer and command pool utility, for custom usage.
    /// </summary>
    public readonly Commands commands;
    /// <summary>
    /// The vulkan instance utility, for custom usage.
    /// </summary>
    public readonly Instance instance;

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
        // TODO: Temporary
        pipeline = new Pipeline(this, Shaders.CreateShader(this, File.ReadAllBytes("examples/HelloTriangle/vert.spv"), File.ReadAllBytes("examples/HelloTriangle/frag.spv"))!);
        // Shaders
        // Pipeline
        swapchain.CreateFramebuffers(renderPass);
        commands = new Commands(this);
        syncronization = new Syncronization(this);
    }
    /// <inheritdoc/>
    public void Render() {
        syncronization.WaitForFrame();
        
        uint imageIndex = swapchain.AcquireNextImage(syncronization);
        commands.Record(
            this, 
            ref swapchain.frameBuffers![imageIndex]
        );
        commands.Submit(this);
        
        device.Present(this, ref imageIndex);

         // NOTE: Can be at start (setup inFlight fence to be signaled).
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