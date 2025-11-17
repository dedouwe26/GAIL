using GAIL.Core;
using GAIL.Graphics.Material;
using GAIL.Graphics.Renderer.Layer;
using LambdaKit.Logging;

namespace GAIL.Graphics.Renderer.Vulkan.Layer;

/// <summary>
/// The Vulkan implementation of the back-end rasterization layer.
/// </summary>
public class VulkanRasterizationLayer : IVulkanLayer, IRasterizationLayer {
    internal VulkanRasterizationLayer(Renderer renderer, uint index, RasterizationLayerSettings settings) {
        Logger = LoggerFactory.CreateSublogger(renderer.Logger, "Layer "+index, "layer"+index);
        Renderer = renderer;
        Index = index;
        this.settings = settings;
        Logger.LogDebug("Creating a Vulkan rasterization back-end.");
		try {
            Pipeline = new Pipeline(this);

			LoadObjects();
		} catch (Exception e) {
            Logger.LogFatal("Exception occured while initializing Vulkan renderer:");
            Logger.LogException(e);
            throw;
        }
	}
    /// <summary>
    /// If the Vulkan rasterization layer is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public Pipeline Pipeline { get; internal set; }

    /// <summary>
    /// The renderer corresponding to this back-end 
    /// </summary>
    public readonly Renderer Renderer;
    /// <summary>
    /// The logger of this back-end 
    /// </summary>
    public readonly Logger Logger;

    #region Settings

    /// <inheritdoc/>
    public CullMode CullMode { get => settings.CullMode; set {
        Pipeline.Dispose();
        settings.CullMode = value;
        Pipeline = new Pipeline(this);
    } }
    /// <inheritdoc/>
    public FillMode FillMode { get => settings.FillMode; set {
        Pipeline.Dispose();
        settings.FillMode = value;
        Pipeline = new Pipeline(this);
    } }
    /// <inheritdoc/>
    public FrontFaceMode FrontFaceMode { get => settings.FrontFaceMode; set {
        Pipeline.Dispose();
        settings.FrontFaceMode = value;
        Pipeline = new Pipeline(this);
    } }
    /// <inheritdoc/>
    public IShader Shader { get => settings.Shader; set {
        Pipeline.Dispose();
        if (value is not Shader shader) {
            throw new InvalidOperationException("The given shader is no Vulkan shader.");
        }
        settings.Shader = shader;
        Pipeline = new Pipeline(this);
    } }
    public bool ShouldRender { get => settings.ShouldRender; set {
        if (settings.ShouldRender != value) {
            settings.ShouldRender = value;
			shouldRecord = true;
		}
    } }
	private readonly RasterizationLayerSettings settings;

	#endregion Settings

	internal uint Index { get; set; }

	private bool shouldRecord;
	public bool ShouldRecord => shouldRecord;

	private RenderSet[] renderSets = [];
    public void UnloadObjects() {
		shouldRecord = true;
        lock (renderSets) {
            foreach (RenderSet renderSet in renderSets) {
                renderSet.Dispose();
            }
        }
    }
    public void LoadObjects() {
		shouldRecord = true;
		lock (renderSets) {
            renderSets = new RenderSet[settings.RenderList.Count];
            for (int i = 0; i < settings.RenderList.Count; i++) {
                renderSets[i] = new(this, settings.RenderList[i]);
            }
        }
    }

    /// <inheritdoc/>
    public void Record(Commands commands) {
        if (!settings.ShouldRender) return;

        commands.BindPipeline(Pipeline);
		commands.SetViewport(ref Renderer.Swapchain.viewport);

		lock (renderSets) {
            foreach (RenderSet renderSet in renderSets) {
				Render(commands, renderSet);
            }
        }

		shouldRecord = false;
	}
    private static void Render(Commands commands, RenderSet renderSet) {
        // obj.material.Apply(); // TODO: Uniforms // NOTE: Applying uniforms.
        commands.BindVertexBuffer(renderSet.vertexBuffer); // NOTE: Applying attributes.
        commands.Draw(vertexCount:3); // TODO: Temporary...
    }

    void IVulkanRecreate() {
		Logger.LogDebug("Recreating a Vulkan rasterization back-end ");
		try {
            Pipeline = new Pipeline(this);

			LoadObjects();
		} catch (Exception e) {
            Logger.LogFatal("Exception occured while initializing Vulkan renderer:");
            Logger.LogException(e);
            throw;
        }
	}
    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) return;

        UnloadObjects();
        Pipeline.Dispose();

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}