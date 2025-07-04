using GAIL.Graphics.Material;
using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Layer;

/// <summary>
/// A layer for 2D graphics.
/// </summary>
public class Layer2D : ILayer {
    public static Layer2D Create(GraphicsManager manager, Object[] initialRenderList, IShader shader) {
        return new(manager.CreateRasterizationLayer(new() { Shaders = shader, RenderList = [.. initialRenderList] }));
    }
    /// <summary>
    /// If this 2D layer is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    // IRasterizationLayer ILayer<IRasterizationLayer>.BackendLayer => backendLayer;

    private IRasterizationLayer backendLayer;
    /// <summary>
    /// Creates a new 2D layer.
    /// </summary>
    public Layer2D(IRasterizationLayer backendLayer) {
        this.backendLayer = backendLayer;
        IsDisposed = false;
    }
    // TODO:
    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="obj"></param>
    // /// <exception cref="NullReferenceException"></exception>
    // public void Render(Object obj) {
    //     if (backendLayer == null) throw new NullReferenceException("Layer is not initialized");
    //     backendLayer.
    // }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) return;

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}