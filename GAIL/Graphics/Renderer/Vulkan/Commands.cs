using System.Reflection;
using GAIL.Core;
using OxDED.Terminal.Logging;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class Commands : IDisposable {
    public bool IsDisposed { get; private set; }
    public readonly CommandPool commandPool;
    public CommandBuffer[] commandBuffers;
    private readonly Device device;
    public Commands(VulkanRenderer renderer) {
        device = renderer.device;

        { // Command pool
            QueueFamilyIndices queueFamilyIndices = device.FindQueueFamilies(device.physicalDevice);

            CommandPoolCreateInfo createInfo = new() {
                SType = StructureType.CommandPoolCreateInfo,

                Flags = CommandPoolCreateFlags.ResetCommandBufferBit, // NOTE: allows command buffers to be rerecorded individually.
                QueueFamilyIndex = queueFamilyIndices.GraphicsFamily!.Value
            };

            renderer.Logger.LogDebug("Creating Command Pool.");
            unsafe {
                _ = Utils.Check(API.Vk.CreateCommandPool(device.logicalDevice, createInfo, Allocator.allocatorPtr, out commandPool), renderer.Logger, "Failed to create commandpool", true);
            }
        }

        commandBuffers = new CommandBuffer[renderer.MaxFramesInFlight];

        // Command buffers
        CommandBufferAllocateInfo allocateInfo = new() {
            SType = StructureType.CommandBufferAllocateInfo,

            CommandPool = commandPool,
            Level = CommandBufferLevel.Primary, // NOTE: Primary: Can be submitted to a queue, secondary: can be called from the primary (e.g. reusability).
            CommandBufferCount = renderer.MaxFramesInFlight
        };

        renderer.Logger.LogDebug("Allocating Command Buffers.");

        commandBuffers = new CommandBuffer[renderer.MaxFramesInFlight];
        unsafe {
            _ = Utils.Check(API.Vk.AllocateCommandBuffers(device.logicalDevice, allocateInfo, Pointer<CommandBuffer>.FromArray(ref commandBuffers)), renderer.Logger, "Failed to allocate command buffer", true);
        }
    }
    
    public void Record(VulkanRenderer renderer, ref Framebuffer swapchainImage) {
        RecordCommandBuffer(renderer, commandBuffers[renderer.CurrentFrame], ref swapchainImage);
    }

    public void Submit(VulkanRenderer renderer) {
        PipelineStageFlags[] waitStages = [PipelineStageFlags.ColorAttachmentOutputBit];
        Silk.NET.Vulkan.Semaphore[] waitSemaphores = [renderer.syncronization.imageAvailable[renderer.CurrentFrame]];
        
        Silk.NET.Vulkan.Semaphore[] signalSemaphores = [renderer.syncronization.renderFinished[renderer.CurrentFrame]];

        SubmitInfo submitInfo = new() {
            SType = StructureType.SubmitInfo,

            WaitSemaphoreCount = 1, // NOTE: Define which semaphores to wait on before execution.
            PWaitSemaphores = Pointer<Silk.NET.Vulkan.Semaphore>.FromArray(ref waitSemaphores),
            PWaitDstStageMask = Pointer<PipelineStageFlags>.FromArray(ref waitStages), // NOTE: Only wait with writing colors, not with the pipeline.
        
            CommandBufferCount = 1, // commandbuffers
            PCommandBuffers = Pointer<CommandBuffer>.From(ref commandBuffers[renderer.CurrentFrame]),

            SignalSemaphoreCount = 1, // NOTE: The semaphores to signal.
            PSignalSemaphores = Pointer<Silk.NET.Vulkan.Semaphore>.FromArray(ref signalSemaphores)
        };

        _ = Utils.Check(API.Vk.QueueSubmit(renderer.device.graphicsQueue, [submitInfo], renderer.syncronization.inFlight[renderer.CurrentFrame]), renderer.Logger, "Failed to submit draw command buffer", true);
    }

    public static void RecordCommandBuffer(VulkanRenderer renderer, CommandBuffer buffer, ref Framebuffer swapchainImage) {
        API.Vk.ResetCommandBuffer(buffer, CommandBufferResetFlags.None);
        
        { // Begin CommandBuffer
            CommandBufferBeginInfo beginInfo = new() {
                SType = StructureType.CommandBufferBeginInfo,
                Flags = 0,
                PInheritanceInfo = Pointer<CommandBufferInheritanceInfo>.FromNull() // NOTE: Only relevant for secondary buffers.
            };

            _ = Utils.Check(API.Vk.BeginCommandBuffer(buffer, beginInfo), renderer.Logger, "Failed to begin recording the command buffer", true);
        }
        
        { // Command Begin RenderPass
            // TODO: Make changable
            ClearValue clearValue = new(new(float32_0:0, float32_1:0,float32_2:0, float32_3: 1));
            // NOTE: Clear values for load operation clear.

            RenderPassBeginInfo renderPassInfo = new() {
                SType = StructureType.RenderPassBeginInfo,

                RenderPass = renderer.renderPass.renderPass,
                Framebuffer = swapchainImage,

                RenderArea = new(new(0, 0), renderer.Swapchain.extent), // NOTE: Shader loads, stores area

                ClearValueCount = 1, // Reference the clear value.
                PClearValues = Pointer<ClearValue>.From(ref clearValue)
            };
            
            API.Vk.CmdBeginRenderPass(buffer, renderPassInfo, SubpassContents.Inline);
            // NOTE: Inline: renderpass setup commands will be embedded in the primary command buffer.
        }

        { // Command Bind Pipeline
            // NOTE: Binding a *graphics* pipeline.
            API.Vk.CmdBindPipeline(buffer, PipelineBindPoint.Graphics, renderer.pipeline.graphicsPipeline);

            // NOTE: The viewport state is specified as a dynamic state.
            Viewport viewport = new() {
                X = 0,
                Y = 0,
                Width = renderer.Swapchain.extent.Width,
                Height = renderer.Swapchain.extent.Height,
                MinDepth = 0,
                MaxDepth = 0
            };
            API.Vk.CmdSetViewport(buffer, 0, 1, in viewport);
        }

        // TODO: Temporary
        API.Vk.CmdDraw(buffer, 3, 1, 0, 0);

        API.Vk.CmdEndRenderPass(buffer);

        _ = Utils.Check(API.Vk.EndCommandBuffer(buffer), renderer.Logger, "Failed to record a command buffer", true);
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