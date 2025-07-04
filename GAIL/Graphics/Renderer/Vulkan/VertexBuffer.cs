using GAIL.Graphics.Mesh;
using GAIL.Graphics.Renderer.Vulkan.Layer;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class VertexBuffer : Buffer {
    private readonly DeviceMemory memory;
    public VertexBuffer(VulkanRasterizationLayer layer, ref byte[] mesh) : base(
        layer.Renderer, (ulong)mesh.LongLength,
        BufferUsageFlags.VertexBufferBit, SharingMode.Exclusive
    ) {
        DeviceMemory? memory = AllocateMemory();
        if (memory == null) {
            layer.Logger.LogError("Failed to allocate vertex buffer memory.");
            throw new Exception("Failed to allocate vertex buffer memory");
        }
        this.memory = memory.Value;

        MapMemory(this.memory, (ulong)mesh.LongLength, ref mesh);
    }
    /// <inheritdoc/>
    public override void Dispose()
    {
        if (IsDisposed) return;
        GC.SuppressFinalize(this);
        FreeMemory(memory);
        base.Dispose();
    }
}