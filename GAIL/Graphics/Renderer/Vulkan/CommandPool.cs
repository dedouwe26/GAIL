using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class CommandPool : IDisposable {
    public bool IsDisposed { get; private set; }
    public Silk.NET.Vulkan.CommandPool commandPool;
    private readonly Device device;
    public CommandPool(VulkanRenderer renderer) {
        device = renderer.device;

        { // Command pool
            QueueFamilyIndices queueFamilyIndices = device.FindQueueFamilies(device.physicalDevice);

            CommandPoolCreateInfo createInfo = new() {
                SType = StructureType.CommandPoolCreateInfo,

                Flags = CommandPoolCreateFlags.ResetCommandBufferBit, // NOTE: allows command buffers to be rerecorded individually.
                QueueFamilyIndex = queueFamilyIndices.GraphicsFamily!.Value
            };

            unsafe {
                _ = Utils.Check(API.Vk.CreateCommandPool(device.logicalDevice, createInfo, Allocator.allocatorPtr, out commandPool), renderer.Logger, "Failed to create commandpool", true);
            }
        }

        // Command buffers
        
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) { return; }

        unsafe {
            API.Vk.DestroyCommandPool(device.logicalDevice, commandPool, Allocator.allocatorPtr);
        }

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}