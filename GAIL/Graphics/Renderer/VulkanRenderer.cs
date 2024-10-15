using GAIL.Core;
using GAIL.Graphics.Renderer.Vulkan;
using OxDED.Terminal.Logging;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// The settings implementation for Vulkan.
/// </summary>
public class VulkanSettings : Settings {
    private readonly VulkanRenderer renderer;
    /// <summary>
    /// Creates an vulkan settings instance.
    /// </summary>
    /// <param name="renderer">The renderer of these settings.</param>
    public VulkanSettings(VulkanRenderer renderer) {
        this.renderer = renderer;
    }
    /// <inheritdoc/>
    public override Color ClearValue { set {
        renderer.Commands.Dispose();
        clearValue = value;
        renderer.Commands = new(renderer);
    } }

    /// <inheritdoc/>
    public override uint MaxFramesInFlight { set {
        renderer.Syncronization.Dispose();
        renderer.Commands.Dispose();
        maxFramesInFlight = value;
        renderer.Commands = new(renderer);
        renderer.Syncronization = new(renderer);
    } }
    /// <inheritdoc/>
    public override bool ShouldRender { set => shouldRender = value; }
}

/// <summary>
/// Represents a renderer that uses the Vulkan Graphics API.
/// </summary>
public class VulkanRenderer : IRenderer<VulkanSettings> {
    /// <summary>
    /// If this class is already disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }
    /// <summary>
    /// The current in flight frame.
    /// </summary>
    /// <remarks>
    /// CurrentFrame cannot be higher than <see cref="Settings.MaxFramesInFlight"/>.
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
    public Syncronization Syncronization { get; internal set; }
    /// <summary>
    /// The vulkan commandbuffer and command pool utility, for custom usage.
    /// </summary>
    public Commands Commands { get; internal set; }
    /// <summary>
    /// The vulkan instance utility, for custom usage.
    /// </summary>
    public readonly Instance instance;
    private readonly Application.Globals globals;
    private readonly VulkanSettings settings;
    /// <inheritdoc/>
    public VulkanSettings Settings { get => settings; }

    /// <summary>
    /// Creates a new Vulkan Renderer.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="globals">The globals of the application.</param>
    /// <param name="appInfo">The application info.</param>
    public VulkanRenderer(Logger logger, Application.Globals globals, AppInfo appInfo) {
        this.globals = globals;
        settings = new(this);

        Logger = logger;
        if (!API.Glfw.VulkanSupported()) {
            Logger.LogFatal("Vulkan: Not Supported!");
            throw new APIBackendException("Vulkan", "Not Supported");
        }

        Logger.LogDebug("Starting Vulkan.");

        instance = new Instance(this, appInfo);
        surface = new Surface(this, globals.windowManager);
        device = new Device(this);
        Swapchain = new SwapChain(this, globals.windowManager);

        renderPass = new RenderPass(this);
        Swapchain.CreateFramebuffers(renderPass);

        // TODO: Temporary
        pipeline = new Pipeline(this, Shaders.CreateShader(this, File.ReadAllBytes("examples/HelloTriangle/vert.spv"), File.ReadAllBytes("examples/HelloTriangle/frag.spv"))!);

        Commands = new Commands(this);
        Syncronization = new Syncronization(this);

        Logger.LogDebug("Done setting up Vulkan.");
    }
    /// <inheritdoc/>
    public void Render() {
        if (!settings.ShouldRender) return;


        Syncronization.WaitForFrame(CurrentFrame);

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

        Syncronization.Reset(CurrentFrame);

        Commands.Record(
            this,
            ref Swapchain.frameBuffers![imageIndex]
        );
        Commands.Submit(this);
        
        if (!device.Present(this, ref imageIndex)) {
            RecreateSwapchain();
        }

        CurrentFrame = (CurrentFrame+1)%settings.MaxFramesInFlight;
    }

    /// <inheritdoc/>
    public void Resize(int width, int height) {
        if (width == 0 || width == 0) {
            settings.ShouldRender = false;
        } else if (!settings.ShouldRender) {
            settings.ShouldRender = true;
        }
        device.shouldRecreateSwapchain = true;
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
        Syncronization.Dispose();
        Commands.Dispose();
        pipeline.Dispose();
        renderPass.Dispose();
        Swapchain.Dispose();
        device.Dispose();
        surface.Dispose();
        instance.Dispose();
        
        GC.SuppressFinalize(this);
    }

    
}