using GAIL.Graphics.Renderer.Vulkan.Layer;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class VertexBuffer : Buffer {
    private readonly DeviceMemory memory;
    public VertexBuffer(VulkanRasterizationLayer layer, Mesh.Mesh mesh) : base(
        layer.Renderer, layer.settings.shader.GetAttributesSize() * (ulong)mesh.vertices.LongLength,
        BufferUsageFlags.VertexBufferBit, SharingMode.Exclusive
    ) {
        DeviceMemory? memory = AllocateMemory();
        if (memory == null) {
            layer.Logger.LogError("Failed to allocate vertex buffer memory.");
            throw new Exception("Failed to allocate vertex buffer memory");
        }
        this.memory = memory.Value;
    }
    public override void Dispose() {
        if (IsDisposed) return;
        GC.SuppressFinalize(this);
        FreeMemory(memory);
        base.Dispose();
    }
}