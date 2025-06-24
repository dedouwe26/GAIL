namespace GAIL.Graphics.Renderer.Vulkan.Layer;

public class Renderable : IDisposable {
    public Object Object;
    public VertexBuffer vertexBuffer;
    public Renderable(VulkanRasterizationLayer layer, Object obj) {
        Object = obj;
        vertexBuffer = new(layer, obj.mesh);
    }
}