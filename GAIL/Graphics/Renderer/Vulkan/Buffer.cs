using GAIL.Core;
using LambdaKit.Logging;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class Buffer : IDisposable {
    public Silk.NET.Vulkan.Buffer buffer;
    public bool IsDisposed { get; private set; } = false;
    private readonly Logger logger;
    private readonly Device device;
    public Buffer(VulkanRenderer renderer, ulong size, BufferUsageFlags usage, SharingMode sharingMode, BufferCreateFlags flags = BufferCreateFlags.None, uint[]? queueFamilyIndices = null) {
        logger = renderer.Logger;
        device = renderer.device;
        BufferCreateInfo createInfo = new() {
            SType = StructureType.BufferCreateInfo,
            Size = size,
            Usage = usage,
            Flags = flags,
            SharingMode = sharingMode
        };
        if (queueFamilyIndices != null) {
            createInfo.QueueFamilyIndexCount = Convert.ToUInt32(queueFamilyIndices.LongLength);
            unsafe {
                createInfo.PQueueFamilyIndices = Pointer<uint>.FromArray(ref queueFamilyIndices);
            }
        }
        unsafe {
            if (Utils.Check(API.Vk.CreateBuffer(device.logicalDevice, in createInfo, Allocator.allocatorPtr, out buffer), renderer.Logger, "Failed to create a buffer")) {
                throw new APIBackendException("Vulkan", "Failed to create a buffer");
            }
        }
    }

    public MemoryRequirements GetMemoryRequirements() {
        API.Vk.GetBufferMemoryRequirements(device.logicalDevice, buffer, out MemoryRequirements requirements);
        return requirements;
    }
    public uint? FindMemoryType(uint typeFilter, MemoryPropertyFlags properties) {
        PhysicalDeviceMemoryProperties memProperties = API.Vk.GetPhysicalDeviceMemoryProperties(device.physicalDevice);
        for (int i = 0; i < memProperties.MemoryTypeCount; i++) {
            if ((typeFilter & (1 << i)) != 0 && (memProperties.MemoryTypes[i].PropertyFlags & properties) == properties) {
                return (uint)i;
            }
        }

        return null;
        
    }
    public DeviceMemory? AllocateMemory(ulong offset = 0) {
        MemoryRequirements memoryRequirements = GetMemoryRequirements();
        uint? type = FindMemoryType(memoryRequirements.MemoryTypeBits, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        if (type == null) {
            logger.LogError("Failed to find suitable memory type for buffer.");
            throw new("Failed to find suitable memory type for buffer");
        }
        MemoryAllocateInfo allocateInfo = new() {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memoryRequirements.Size,
            MemoryTypeIndex = type.Value
        };
        unsafe {
            if (!Utils.Check(API.Vk.AllocateMemory(device.logicalDevice, in allocateInfo, Allocator.allocatorPtr, out DeviceMemory memory), logger, "Failed to allocate buffer memory")) {
                return null;
            }
            if (!Utils.Check(API.Vk.BindBufferMemory(device.logicalDevice, buffer, memory, offset), logger, "Failed to bind memory to buffer")) {
                FreeMemory(memory);
                return null;
            }
            return memory;
        }

    }
    public unsafe void FreeMemory(DeviceMemory memory) {
        API.Vk.FreeMemory(device.logicalDevice, memory, Allocator.allocatorPtr);
    }
    public unsafe bool MapMemory(DeviceMemory memory, ulong size, ref byte[] data, ulong offset = 0, MemoryMapFlags flags = MemoryMapFlags.None) {
        logger.LogDebug("Mapping memory...");
        void* rawData;
        if (!Utils.Check(API.Vk.MapMemory(device.logicalDevice, memory, offset, size, flags, &rawData), logger, "Failed to map memory")) return false;
        data.AsSpan().CopyTo(new Span<byte>(rawData, data.Length));
        API.Vk.UnmapMemory(device.logicalDevice, memory);
        return true;
    }

    public virtual void Dispose() {
        if (IsDisposed) return;
        GC.SuppressFinalize(this);

        unsafe {
            API.Vk.DestroyBuffer(device.logicalDevice, buffer, Allocator.allocatorPtr);
        }

        IsDisposed = true;
    }
}
