using GAIL.Graphics.Material;
using GAIL.Graphics.Renderer.Layer;
using LambdaKit.Logging;

namespace GAIL.Graphics.Renderer.Vulkan.Layer;

/// <summary>
/// Vulkan implementation of the rasterization layer settings.
/// </summary>
public class VulkanRasterizationLayerSettings : RasterizationLayerSettings<VulkanRasterizationLayer> {
    /// <summary>
    /// Creates new a new Vulkan implementation of the rasterization layer settings.
    /// </summary>
    /// <param name="layer">The vulkan implementation of the rasterization layer.</param>
    /// <param name="values">The initial values of these settings.</param>
    public VulkanRasterizationLayerSettings(VulkanRasterizationLayer layer, RasterizationLayerSettings values) : base(layer, values) {
        if (values.Shaders is not Shader shader) {
            throw new InvalidOperationException("The given shader is no Vulkan shader.");
        }
        this.shader = shader;
    }
    
    /// <inheritdoc/>
    public override CullMode CullMode { get => base.CullMode; set {
        layer.Pipeline.Dispose();
        cullMode = value;
        layer.Pipeline = new Pipeline(layer);
    } }
    /// <inheritdoc/>
    public override FillMode FillMode { get => base.FillMode; set {
        layer.Pipeline.Dispose();
        fillMode = value;
        layer.Pipeline = new Pipeline(layer);
    } }
    /// <inheritdoc/>
    public override FrontFaceMode FrontFaceMode { get => base.FrontFaceMode; set {
        layer.Pipeline.Dispose();
        frontFaceMode = value;
        layer.Pipeline = new Pipeline(layer);
    } }
    /// <inheritdoc/>
    public override IShader Shader { get => shader; set {
        layer.Pipeline.Dispose();
        if (value is not Shader shader) {
            throw new InvalidOperationException("The given shader is no Vulkan shader.");
        }
        this.shader = shader;
        layer.Pipeline = new Pipeline(layer);
    } }
    internal Shader shader;
    /// <inheritdoc/>
    public override List<Object> RenderList { get => base.RenderList; set {
        layer.UnloadObjects(); // TODO: FIXME: This is inefficient when a new object is added.
        renderList = value;
        layer.LoadObjects(); 
    } }
}

/// <summary>
/// The Vulkan implementation of the back-end rasterization layer.
/// </summary>
public class VulkanRasterizationLayer : IVulkanLayer, IRasterizationLayer {
    internal VulkanRasterizationLayer(VulkanRenderer renderer, uint index, RasterizationLayerSettings settings) {
        Logger = renderer.Logger.CreateSubLogger("layer"+index, "Rasterization Layer");
        Renderer = renderer;
        Index = index;
        this.settings = new(this, settings);
#if DEBUG
        Logger.LogDebug("Creating a Vulkan rasterization back-end layer.");
#endif
        Pipeline = new Pipeline(this);
    }
    /// <summary>
    /// If the Vulkan rasterization layer is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public Pipeline Pipeline { get; internal set; }

    /// <summary>
    /// The renderer corresponding to this back-end layer.
    /// </summary>
    public readonly VulkanRenderer Renderer;
    /// <summary>
    /// The logger of this back-end layer.
    /// </summary>
    public readonly Logger Logger;
    internal readonly VulkanRasterizationLayerSettings settings;
    /// <inheritdoc/>
    public IRasterizationLayerSettings Settings => settings;
    internal uint Index;
    Pipeline IVulkanLayer.Pipeline { get => Pipeline; set => Pipeline = value; }
    uint IVulkanLayer.Index { set {
        Pipeline.Dispose();
        Index = value;
        Pipeline = new(this);
    } }

    private Renderable[] renderables = [];
    public void UnloadObjects() {
        lock (renderables) {
            foreach (Renderable renderable in renderables) {
                renderable.Dispose();
            }
        }
    }
    public void LoadObjects() {
        lock (renderables) {
            renderables = new Renderable[settings.RenderList.Count];
            for (int i = 0; i < settings.RenderList.Count; i++) {
                renderables[i] = new(this, settings.RenderList[i]);
            }
        }
    }

    /// <inheritdoc/>
    public void Render(Commands commands) {
        if (!settings.ShouldRender) return;

        RecordCommands(commands);
    }
    private void RecordCommands(Commands commands) {
        commands.BindPipeline(Pipeline);

        lock (renderables) {
            foreach (Renderable obj in renderables) {
                Render(commands, obj);
            }
        }
    }
    private void Render(Commands commands, Renderable renderable) {
        // obj.material.Apply(); // TODO: Uniforms // NOTE: Applying uniforms.
        commands.BindVertexBuffer(renderable.vertexBuffer); // NOTE: Applying attributes.
        commands.Draw(vertexCount:3); // TODO: Temporary...
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) return;

        Pipeline.Dispose();

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}