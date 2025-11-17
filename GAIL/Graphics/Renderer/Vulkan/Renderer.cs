using GAIL.Core;
using GAIL.Graphics.Material;
using GAIL.Graphics.Renderer.Layer;
using GAIL.Graphics.Renderer.Vulkan.Layer;
using GAIL.Window;
using LambdaKit.Logging;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

/// <summary>
/// Represents a renderer that uses the Vulkan Graphics API.
/// </summary>
public class Renderer : IRenderer {
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
	internal LayerDescription[] layerDescriptions;
	private IVulkanLayer[] layers;
	internal Logger Logger;

	#region Utilities

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
	public Swapchain Swapchain { get; private set; }
	/// <summary>
	/// The vulkan renderpass utility, for custom usage.
	/// </summary>
	public RenderPass? RenderPass { get; internal set; } // TODO: Temp
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

	#endregion Utilities

	#region Settings

	/// <inheritdoc/>
	public IVulkanLayer[] Layers { get => settings.Layers; set { // NOTE: Casting because this is garanteed.
		for (uint i = 0; i < value.Length; i++) {
			value[i].Index = i;
		}

		RenderPass?.Dispose();
		settings.Layers = value;
		layerDescriptions = LayerDescription.From(value);
		RenderPass = new(this);
		RenderPass.CreateFramebuffers();
	} }
	/// <inheritdoc/>
	public Color ClearValue { get => settings.ClearValue; set => settings.ClearValue = value; } // TODO: Only thing to change?
	/// <inheritdoc/>
	public uint MaxFramesInFlight { get => settings.MaxFramesInFlight; set {
		// TODO: ??? device wait idle at all settings (including layer settings) ???
		Syncronization.Dispose();
		Commands.Dispose();
		settings.MaxFramesInFlight = value;
		Commands = new(this);
		Syncronization = new(this);
	} }
	/// <inheritdoc/>
	public bool ShouldRender { get; set; }
	private readonly RendererSettings settings;

	#endregion Settings

	internal readonly WindowManager windowManager;

	/// <summary>
	/// Creates a new Vulkan Renderer.
	/// </summary>
	/// <param name="logger">The logger to use.</param>
	/// <param name="windowManager">The window manager to use.</param>
	/// <param name="settings">The initial settings of this renderer.</param>
	/// <param name="appInfo">The information of the application.</param>
	public Renderer(Logger logger, WindowManager windowManager, RendererSettings settings, AppInfo appInfo) {
		this.windowManager = windowManager;
		this.settings = settings;

		Logger = logger;
		if (!API.Glfw.VulkanSupported()) {
			Logger.LogError("Vulkan: Not Supported!");
			throw new APIBackendException("Vulkan", "Not Supported");
		}

		Logger.LogDebug("Initializing Vulkan");

		layerDescriptions = LayerDescription.From(settings.LayerSettings);
		try {
			instance = new Instance(this, appInfo);
			surface = new Surface(this);
			device = new Device(this);
			Swapchain = new Swapchain(this);

			if (RenderPass != null) {
				RenderPass = new RenderPass(this); // TODO: Does this work?
				RenderPass.CreateFramebuffers();   // NOTE: This is here, because creating a rasterization layer also initializes the renderpass.
			}

			// TODO: CreateLayers

			Commands = new Commands(this);
			Syncronization = new Syncronization(this);
		} catch (Exception e) { // TODO: Better exception handling.
			Logger.LogFatal("Exception occured while initializing Vulkan renderer:");
			Logger.LogException(e);
			throw;
		}

		Logger.LogDebug("Done setting up the Renderer.");
	}
	/// <inheritdoc/>
	public void Render() {
		if (!settings.ShouldRender) return;

		Syncronization.WaitForFrame(CurrentFrame);

		if (!Swapchain.AcquireNextImage(this, out uint imageIndex)) {
			RecreateSwapchain();
			return;
		}

		Syncronization.Reset(CurrentFrame);

		// TODO: Add layers to render pass?
		// TODO: RenderFinished semaphores?

		Commands.Update();

		if (ShouldRecord() || Commands.IsCommandBufferInitial()) {
			Commands.BeginRecord(RenderPass!.Framebuffers![imageIndex]);
		
			{
				//   <<< LAYER SPECIFICS >>>

				foreach (IVulkanLayer backendLayer in settings.Layers) {
					backendLayer.Record(Commands);
				}

				// <<< END LAYER SPECIFICS >>>
			}
			
			Commands.EndRecord();
		}
		
		Commands.Submit();

		if (!device.Present(imageIndex)) {
			RecreateSwapchain();
		}

		CurrentFrame = (CurrentFrame+1)%settings.MaxFramesInFlight;
	}

	/// <inheritdoc/>
	public void Resize(int width, int height) {
		if (width == 0 || height == 0) {
			settings.ShouldRender = false;
		} else if (!settings.ShouldRender) {
			settings.ShouldRender = true;
		}
		RecreateSwapchain();
	}
	private bool ShouldRecord() {
		foreach (IVulkanLayer backendLayer in settings.Layers) {
			if (backendLayer.ShouldRecord) {
				return true;
			}
		}
		return false;
	}
	internal void RecreateSwapchain() {
		device.WaitIdle();

		RenderPass?.DisposeFramebuffers();
		foreach (IVulkanLayer layer in settings.Layers) {
			layer.Dispose();
		}
		RenderPass?.Dispose();
		Swapchain.Dispose();

		Swapchain = new Swapchain(this, globals.windowManager);
		if (settings.Layers.Length > 0) RenderPass = new(this);
		RenderPass?.CreateFramebuffers();
		foreach (IVulkanLayer layer in settings.Layers) {
			layer.Recreate();
		}
	}
	/// <inheritdoc/>
	public void Dispose() {
		if (IsDisposed) { return; }

		device.WaitIdle();

		Commands.Dispose();

		Logger.LogDebug("Terminating Vulkan.");
		foreach (IVulkanLayer backendLayer in settings.Layers) {
			backendLayer.Dispose();
		}
		
		RenderPass?.Dispose();
		Swapchain.Dispose();
		device.Dispose();
		surface.Dispose();
		instance.Dispose();
		
		GC.SuppressFinalize(this);
	}

	#region API

	/// <inheritdoc/>
	public IRasterizationLayer? CreateRasterizationLayer(RasterizationLayerSettings settings) {
		RenderPass?.Dispose(); // TODO: Distinct between creating a layer and appending it.
		layerDescriptions = [.. layerDescriptions, new() { type = PipelineBindPoint.Graphics }];
		RenderPass = new(this);
		RenderPass.CreateFramebuffers();
		VulkanRasterizationLayer layer = new(this, (uint)this.settings.Layers.Length, settings);
		this.settings.Layers = [.. this.settings.Layers, layer];
		try {
			return layer;
		} catch (APIBackendException) {
			Logger.LogError("Failed to create rasterization layer.");
			return default;
		}
	}
	/// <inheritdoc/>
	/// <exception cref="APIBackendException"/>
	public IShader? CreateShader(FormatInfo[] requiredAttributes, FormatInfo[] requiredUniforms, byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null) {
		try {
			return Shader.Create(this, requiredAttributes, requiredUniforms, vertexShader, fragmentShader, geometryShader);
		} catch (APIBackendException) {
			Logger.LogError("Failed to create a shader.");
			return default;
		}
	}

	#endregion API
}