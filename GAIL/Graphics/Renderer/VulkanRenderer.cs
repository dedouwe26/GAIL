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
    /// <summary>
    /// The maximum amount of frames that can be rendered at a time.
    /// </summary>
    public uint MaxFramesInFlight { get; private set; }
    /// <summary>
    /// The current in flight frame.
    /// </summary>
    /// <remarks>
    /// CurrentFrame cannot be higher than <see cref="MaxFramesInFlight"/>.
    /// </remarks>
    public uint CurrentFrame { get; private set; }
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
    public SwapChain Swapchain { get; private set; }
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
    private readonly Application.Globals globals;

    /// <summary>
    /// Creates a new Vulkan Renderer.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="globals">The globals of the application.</param>
    /// <param name="appInfo">The application info.</param>
    public VulkanRenderer(Logger logger, Application.Globals globals, AppInfo appInfo) {
        this.globals = globals;

        Logger = logger;
        if (!API.Glfw.VulkanSupported()) {
            Logger.LogFatal("Vulkan: Not Supported!");
            throw new APIBackendException("Vulkan", "Not Supported");
        }

        // TODO: Temporary
        MaxFramesInFlight = 4;

        Logger.LogDebug("Starting Vulkan.");

        instance = new Instance(this, appInfo);
        surface = new Surface(this, globals.windowManager);
        device = new Device(this);
        Swapchain = new SwapChain(this, globals.windowManager);

        renderPass = new RenderPass(this);

        // TODO: Temporary
        pipeline = new Pipeline(this, Shaders.CreateShader(this, File.ReadAllBytes("examples/HelloTriangle/vert.spv"), File.ReadAllBytes("examples/HelloTriangle/frag.spv"))!);

        Swapchain.CreateFramebuffers(renderPass);
        commands = new Commands(this);
        syncronization = new Syncronization(this);

        Logger.LogDebug("Done setting up Vulkan.");
    }
    /// <inheritdoc/>
    public void Render() {
        syncronization.WaitForFrame(CurrentFrame);

        uint imageIndex;
        {
            uint? val;

            if ((val = Swapchain.AcquireNextImage(this)) == null) {
                RecreateSwapchain();
                return;
            } else {
                imageIndex = val.Value;
            }
        }

        syncronization.Reset(CurrentFrame);

        commands.Record(
            this,
            ref Swapchain.frameBuffers![imageIndex]
        );
        commands.Submit(this);
        
        if (!device.Present(this, ref imageIndex)) {
            RecreateSwapchain();
        }

        CurrentFrame = (CurrentFrame+1)%MaxFramesInFlight;
    }
    /// <inheritdoc/>
    public void UpdateSettings(Settings newSettings) {

    }

    /// <inheritdoc/>
    public void Resize() {
        RecreateSwapchain();
    }

    
    private void RecreateSwapchain() {
        device.WaitIdle();

        Swapchain.Dispose();

        Swapchain = new SwapChain(this, globals.windowManager);
        Swapchain.CreateFramebuffers(renderPass);
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) { return; }

        device.WaitIdle();
        
        Logger.LogDebug("Terminating Vulkan.");
        syncronization.Dispose();
        commands.Dispose();
        pipeline.Dispose();
        renderPass.Dispose();
        Swapchain.Dispose();
        device.Dispose();
        surface.Dispose();
        instance.Dispose();
        
        GC.SuppressFinalize(this);
    }

    
}