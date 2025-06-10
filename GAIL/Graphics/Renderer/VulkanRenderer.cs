using GAIL.Core;
using GAIL.Graphics.Material;
using GAIL.Graphics.Renderer.Layer;
using GAIL.Graphics.Renderer.Vulkan;
using GAIL.Graphics.Renderer.Vulkan.Layer;
using LambdaKit.Logging;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// The settings implementation for Vulkan.
/// </summary>
public class VulkanSettings : RendererSettings<VulkanRenderer, IVulkanLayer> {
    /// <summary>
    /// Creates new a new Vulkan implementation of the rasterization layer settings.
    /// </summary>
    /// <param name="renderer">The vulkan implementation of the renderer settings.</param>
    /// <param name="values">The initial values of these settings.</param>
    public VulkanSettings(VulkanRenderer renderer, ref RendererSettings<IVulkanLayer> values) : base(renderer, ref values) { }

    /// <inheritdoc/>
    public override uint MaxFramesInFlight { get => base.MaxFramesInFlight; set {
        // TODO: ??? device wait idle at all settings (including layer settings) ???
        renderer.Syncronization.Dispose();
        renderer.Commands.Dispose();
        maxFramesInFlight = value;
        renderer.Commands = new(renderer);
        renderer.Syncronization = new(renderer);
    } }
    /// <inheritdoc/>
    public override Color ClearValue { get => base.ClearValue; set => clearValue = value; }
    /// <inheritdoc/>
    public override IVulkanLayer[] Layers { get => base.Layers; set {
        for (uint i = 0; i < value.Length; i++) {
            value[i].Index = i;
        }

        renderer.RenderPass.Dispose();
        renderer.Swapchain.DisposeFramebuffers();
        layers = value;
        renderer.RenderPass = new(renderer);
        renderer.Swapchain.CreateFramebuffers(renderer.RenderPass);
    } }
}

/// <summary>
/// Represents a renderer that uses the Vulkan Graphics API.
/// </summary>
public class VulkanRenderer : IRenderer<IVulkanLayer> {
    /// <summary>
    /// If this class is already disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }
    /// <summary>
    /// The current in flight frame.
    /// </summary>
    /// <remarks>
    /// CurrentFrame cannot be higher than <see cref="IRendererSettings.MaxFramesInFlight"/>.
    /// </remarks>
    public uint CurrentFrame { get; private set; }
    /// <summary>
    /// The image index in the swap chain.
    /// </summary>
    public uint ImageIndex { get; private set; }
    internal Logger Logger;

    # region Utilities

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
    public RenderPass RenderPass { get; internal set; }
    /// <summary>
    /// The Vulkan syncronization utility, for custom usage.
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

    # endregion Utilities

    private readonly Application.Globals globals;
    private readonly VulkanSettings settings;
    /// <inheritdoc/>
    public IRendererSettings<IVulkanLayer> Settings { get => settings; }

    /// <summary>
    /// Creates a new Vulkan Renderer.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="globals">The globals of the application.</param>
    /// <param name="settings">The settings of this renderer.</param>
    /// <param name="appInfo">The information of the application.</param>
    public VulkanRenderer(Logger logger, Application.Globals globals, ref RendererSettings<IVulkanLayer> settings, ref AppInfo appInfo) {
        this.globals = globals;
        this.settings = new(this, ref settings);

        Logger = logger;
        if (!API.Glfw.VulkanSupported()) {
            Logger.LogFatal("Vulkan: Not Supported!");
            throw new APIBackendException("Vulkan", "Not Supported");
        }

        Logger.LogDebug("Initializing Vulkan");

        instance = new Instance(this, ref appInfo);
        surface = new Surface(this, globals.windowManager);
        device = new Device(this);
        Swapchain = new SwapChain(this, globals.windowManager);

        RenderPass = new RenderPass(this);
        Swapchain.CreateFramebuffers(RenderPass);
        Commands = new Commands(this);
        Syncronization = new Syncronization(this);

        Logger.LogDebug("Done setting up Vulkan.");
    }
    /// <inheritdoc/>
    public void Render() {
        if (!settings.ShouldRender) return;

        Syncronization.WaitForFrame(CurrentFrame);

        uint imageIndex; {
            uint? val;

            if ((val = Swapchain.AcquireNextImage(this)) == null) {
                RecreateSwapchain();
                return;
            } else {
                imageIndex = val.Value;
            }
        }

        Syncronization.Reset(CurrentFrame);

        // TODO: Add layers to render pass?
        // TODO: RenderFinished semaphores 

        // TODO: Optimization: Don't re-record every frame.
        Commands.BeginRecord(this, ref Swapchain.frameBuffers![imageIndex]);
        
        {
            //   <<< LAYER SPECIFICS >>>

            foreach (IVulkanLayer backendLayer in settings.Layers) {
                backendLayer.Render(Commands);
            }

            // <<< END LAYER SPECIFICS >>>
        }
        
        Commands.EndRecord(this);
        
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
        Swapchain.CreateFramebuffers(RenderPass);
    }

    /// <inheritdoc/>
    public IRasterizationLayer? CreateRasterizationLayer(RasterizationLayerSettings settings) {
        try {
            return new VulkanRasterizationLayer(this, 0, settings);
        } catch (APIBackendException) {
            Logger.LogError("Failed to create rasterization layer.");
            return default;
        }
    }
    /// <inheritdoc/>
    /// <exception cref="APIBackendException"/>
    public IShader CreateShader(byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null) {

    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) { return; }

        device.WaitIdle();

        Commands.Dispose();

        Logger.LogDebug("Terminating Vulkan.");
        foreach (IVulkanLayer backendLayer in Settings.Layers) {
            backendLayer.Dispose();
        }
        
        RenderPass.Dispose();
        Swapchain.Dispose();
        device.Dispose();
        surface.Dispose();
        instance.Dispose();
        
        GC.SuppressFinalize(this);
    }
}