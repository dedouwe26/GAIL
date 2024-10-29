using OxDED.Terminal.Logging;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class Syncronization : IDisposable {
    private readonly Device device;
    private readonly Logger Logger;
    
    /// <summary>
    /// The render finished semaphores, for custom usage.
    /// </summary>
    public readonly Silk.NET.Vulkan.Semaphore[] renderFinished;
    /// <summary>
    /// The image available semaphores, for custom usage.
    /// </summary>    
    public readonly Silk.NET.Vulkan.Semaphore[] imageAvailable;
    /// <summary>
    /// The frame in flight fences, for custom usage.
    /// </summary>
    public readonly Fence[] inFlight;
    public bool IsDisposed { get; private set; }
    public Syncronization(VulkanRenderer renderer) {
        device = renderer.device;
        Logger = renderer.Logger;
        
        renderer.Logger.LogDebug("Creating Sync Objects.");
        
        renderFinished = CreateSemaphore(renderer.Settings.MaxFramesInFlight);
        imageAvailable = CreateSemaphore(renderer.Settings.MaxFramesInFlight);
        inFlight = CreateFence(renderer.Settings.MaxFramesInFlight, true);
    }
    public void WaitForFrame(uint currentFrame) {
        _ = Utils.Check(API.Vk.WaitForFences(device.logicalDevice, 1, in inFlight[currentFrame], true, ulong.MaxValue), Logger, "Failed to wait for in flight fence", true);
    }
    public void Reset(uint currentFrame) {
        _ = Utils.Check(API.Vk.ResetFences(device.logicalDevice, 1, in inFlight[currentFrame]), Logger, "Failed to reset in flight fence", true);
    }
    public Silk.NET.Vulkan.Semaphore[] CreateSemaphore(uint size) {
        Silk.NET.Vulkan.Semaphore[] semaphores = new Silk.NET.Vulkan.Semaphore[size];

        SemaphoreCreateInfo createInfo = new() {
            SType = StructureType.SemaphoreCreateInfo,
            Flags = SemaphoreCreateFlags.None
        };

        for (int i = 0; i < size; i++) {
            unsafe {
                _ = Utils.Check(API.Vk.CreateSemaphore(device.logicalDevice, in createInfo, Allocator.allocatorPtr, out semaphores[i]), Logger, "Failed to create semaphore", true);
            }
        }

        return semaphores;
    }
    public Fence[] CreateFence(uint size, bool signaled = false) {
        Fence[] fences = new Fence[size];

        FenceCreateInfo createInfo = new() {
            SType = StructureType.FenceCreateInfo,
            Flags = signaled ? FenceCreateFlags.SignaledBit : FenceCreateFlags.None
        };

        if (signaled) createInfo.Flags = FenceCreateFlags.SignaledBit;

        for (int i = 0; i < size; i++) {
            unsafe {
                _ = Utils.Check(API.Vk.CreateFence(device.logicalDevice, in createInfo, Allocator.allocatorPtr, out fences[i]), Logger, "Failed to create fence", true);
            }
        }
        
        return fences;
    }
    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) { return; }

        unsafe {
            foreach (Silk.NET.Vulkan.Semaphore semaphore in renderFinished) {
                API.Vk.DestroySemaphore(device.logicalDevice, semaphore, Allocator.allocatorPtr);
            }
            foreach (Silk.NET.Vulkan.Semaphore semaphore in imageAvailable) {
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