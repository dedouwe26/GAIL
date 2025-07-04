namespace GAIL.Graphics.Renderer.Vulkan.Layer;

public class RenderSet : IDisposable {
    private bool isDisposed = false;
    public Object Object;
    public VertexBuffer vertexBuffer;
    public RenderSet(VulkanRasterizationLayer layer, Object obj) {
        Object = obj;
        byte[] bakedMesh = obj.mesh.Bake(layer.settings.Shader);
        vertexBuffer = new(layer, ref bakedMesh);
    }
    public void Dispose() {
        if (!isDisposed) return;
        GC.SuppressFinalize(this);
        vertexBuffer.Dispose();
        isDisposed = true;
    }
}