using OxDED.Terminal.Logging;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class Syncronization : IDisposable {
    private readonly Device device;
    private readonly Logger Logger;
    
    public readonly Silk.NET.Vulkan.Semaphore imageAvailable;
    public readonly Silk.NET.Vulkan.Semaphore renderFinished;
    public readonly Fence inFlight;
    public bool IsDisposed { get; private set; }
    public Syncronization(VulkanRenderer renderer) {
        device = renderer.device;
        Logger = renderer.Logger;

        imageAvailable = CreateSemaphore();
        renderFinished = CreateSemaphore();
        inFlight = CreateFence(false);
    }
    public void WaitForFrame() {
        API.Vk.WaitForFences(device.logicalDevice, 1, in inFlight, true, ulong.MaxValue);
        API.Vk.ResetFences(device.logicalDevice, 1, in inFlight);
    }
    public Fence CreateFence(bool signaled = false) {
        FenceCreateInfo createInfo = new() {
            SType = StructureType.FenceCreateInfo
        };

        if (signaled) createInfo.Flags = FenceCreateFlags.SignaledBit;

        unsafe {
            _ = Utils.Check(API.Vk.CreateFence(device.logicalDevice, createInfo, Allocator.allocatorPtr, out Fence fence), Logger, "Failed to create semaphore", true);
        
            return fence;
        }
    }
    public Silk.NET.Vulkan.Semaphore CreateSemaphore() {
        SemaphoreCreateInfo createInfo = new() {
            SType = StructureType.SemaphoreCreateInfo
        };

        unsafe {
            _ = Utils.Check(API.Vk.CreateSemaphore(device.logicalDevice, createInfo, Allocator.allocatorPtr, out Silk.NET.Vulkan.Semaphore semaphore), Logger, "Failed to create semaphore", true);
        
            return semaphore;
        }
    }
    public void Dispose() {
        if (IsDisposed) { return; }

        unsafe {
            API.Vk.DestroySemaphore(device.logicalDevice, imageAvailable, Allocator.allocatorPtr);
            API.Vk.DestroySemaphore(device.logicalDevice, renderFinished, Allocator.allocatorPtr);
            API.Vk.DestroyFence(device.logicalDevice, inFlight, Allocator.allocatorPtr);
        }

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}