using OxDED.Terminal.Logging;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class Syncronization : IDisposable {
    private readonly Device device;
    private readonly Logger Logger;
    
    public readonly Silk.NET.Vulkan.Semaphore[] imageAvailable;
    public readonly Silk.NET.Vulkan.Semaphore[] renderFinished;
    public readonly Fence[] inFlight;
    public bool IsDisposed { get; private set; }
    public Syncronization(VulkanRenderer renderer) {
        device = renderer.device;
        Logger = renderer.Logger;
        
        renderer.Logger.LogDebug("Creating Sync Objects.");

        imageAvailable = CreateSemaphore(renderer.Settings.MaxFramesInFlight);
        renderFinished = CreateSemaphore(renderer.Settings.MaxFramesInFlight);
        inFlight = CreateFence(renderer.Settings.MaxFramesInFlight, true);
    }
    public void WaitForFrame(uint currentFrame) {
        API.Vk.WaitForFences(device.logicalDevice, 1, in inFlight[currentFrame], true, ulong.MaxValue);
    }
    public void Reset(uint currentFrame) {
        API.Vk.ResetFences(device.logicalDevice, 1, in inFlight[currentFrame]);
    }
    public Fence[] CreateFence(uint size, bool signaled = false) {
        Fence[] fences = new Fence[size];

        FenceCreateInfo createInfo = new() {
            SType = StructureType.FenceCreateInfo
        };

        if (signaled) createInfo.Flags = FenceCreateFlags.SignaledBit;

        for (int i = 0; i < size; i++) {
            unsafe {
                _ = Utils.Check(API.Vk.CreateFence(device.logicalDevice, createInfo, Allocator.allocatorPtr, out fences[i]), Logger, "Failed to create fence", true);
            }
        }
        
        return fences;
    }
    public Silk.NET.Vulkan.Semaphore[] CreateSemaphore(uint size) {
        Silk.NET.Vulkan.Semaphore[] semaphores = new Silk.NET.Vulkan.Semaphore[size];

        SemaphoreCreateInfo createInfo = new() {
            SType = StructureType.SemaphoreCreateInfo
        };

        for (int i = 0; i < size; i++) {
            unsafe {
                _ = Utils.Check(API.Vk.CreateSemaphore(device.logicalDevice, createInfo, Allocator.allocatorPtr, out semaphores[i]), Logger, "Failed to create semaphore", true);
            }
        }

        return semaphores;
    }
    public void Dispose() {
        if (IsDisposed) { return; }

        unsafe {
            foreach (Silk.NET.Vulkan.Semaphore semaphore in imageAvailable) {
                API.Vk.DestroySemaphore(device.logicalDevice, semaphore, Allocator.allocatorPtr);
            }
            foreach (Silk.NET.Vulkan.Semaphore semaphore in renderFinished) {
                API.Vk.DestroySemaphore(device.logicalDevice, semaphore, Allocator.allocatorPtr);
            }
            foreach (Fence fence in inFlight) {
                API.Vk.DestroyFence(device.logicalDevice, fence, Allocator.allocatorPtr);
            }
        }

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}