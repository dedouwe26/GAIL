using GAIL.Core;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class Commands : IDisposable {
    public bool IsDisposed { get; private set; }
    public readonly CommandPool commandPool;
    public CommandBuffer[] commandBuffers;
    private readonly Device device;
    private CommandBuffer currentCommandBuffer;
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
                if (!Utils.Check(API.Vk.CreateCommandPool(device.logicalDevice, in createInfo, Allocator.allocatorPtr, out commandPool), renderer.Logger, "Failed to create commandpool", false)) {
                    throw new APIBackendException("Vulkan", "Failed to create commandpool");
                }
            }
        }

        commandBuffers = new CommandBuffer[renderer.Settings.MaxFramesInFlight];

        // Command buffers
        CommandBufferAllocateInfo allocateInfo = new() {
            SType = StructureType.CommandBufferAllocateInfo,

            CommandPool = commandPool,
            Level = CommandBufferLevel.Primary, // NOTE: Primary: Can be submitted to a queue, secondary: can be called from the primary (e.g. reusability).
            CommandBufferCount = renderer.Settings.MaxFramesInFlight
        };

        renderer.Logger.LogDebug("Allocating Command Buffers.");

        commandBuffers = new CommandBuffer[renderer.Settings.MaxFramesInFlight];
        unsafe {
            if (!Utils.Check(API.Vk.AllocateCommandBuffers(device.logicalDevice, in allocateInfo, Pointer<CommandBuffer>.FromArray(ref commandBuffers)), renderer.Logger, "Failed to allocate command buffer", false)) {
                throw new APIBackendException("Vulkan", "Failed to allocate command buffer");
            }
        }
    }

    public void Submit(VulkanRenderer renderer) {
        PipelineStageFlags[] waitStages = [PipelineStageFlags.ColorAttachmentOutputBit];
        Silk.NET.Vulkan.Semaphore[] waitSemaphores = [renderer.Syncronization.imageAvailable[renderer.CurrentFrame]];
        
        Silk.NET.Vulkan.Semaphore[] signalSemaphores = [renderer.Syncronization.renderFinished[renderer.CurrentFrame]];

        SubmitInfo submitInfo = new() {
            SType = StructureType.SubmitInfo,

            WaitSemaphoreCount = Convert.ToUInt32(waitSemaphores.LongLength), // NOTE: Define which semaphores to wait on before execution.
            PWaitSemaphores = Pointer<Silk.NET.Vulkan.Semaphore>.FromArray(ref waitSemaphores),
            PWaitDstStageMask = Pointer<PipelineStageFlags>.FromArray(ref waitStages), // NOTE: Only wait with writing colors, not with the pipeline.
        
            CommandBufferCount = 1, // commandbuffers
            PCommandBuffers = Pointer<CommandBuffer>.From(ref currentCommandBuffer),

            SignalSemaphoreCount = Convert.ToUInt32(signalSemaphores.LongLength), // NOTE: The semaphores to signal.
            PSignalSemaphores = Pointer<Silk.NET.Vulkan.Semaphore>.FromArray(ref signalSemaphores)
        };

        _ = Utils.Check(API.Vk.QueueSubmit(renderer.device.graphicsQueue, [submitInfo], renderer.Syncronization.inFlight[renderer.CurrentFrame]), renderer.Logger, "Failed to submit draw command buffer", true);
    }
    
    public void BeginRecord(VulkanRenderer renderer, ref Framebuffer swapchainImage) {
        currentCommandBuffer = commandBuffers[renderer.CurrentFrame];
        BeginCommandBuffer(renderer, currentCommandBuffer, ref swapchainImage);
    }

    public void BindPipeline(Layer.Pipeline pipeline) {
        API.Vk.CmdBindPipeline(currentCommandBuffer, pipeline.Type, pipeline.graphicsPipeline);
    }
    public unsafe void BindVertexBuffer(VertexBuffer vertexBuffer) {
        ulong[] offsets = [0];
        fixed (ulong* offsetsPtr = offsets) {
            API.Vk.CmdBindVertexBuffers(currentCommandBuffer, 0, 1, in vertexBuffer.buffer, offsetsPtr);
        }
    }
    public void Draw(uint vertexCount, uint instanceCount = 1, uint firstVertex = 0, uint firstInstance = 0) {
        API.Vk.CmdDraw(currentCommandBuffer, vertexCount, instanceCount, firstVertex, firstInstance);
    }

    public void EndRecord(VulkanRenderer renderer) {
        EndCommandBuffer(renderer, currentCommandBuffer);
    }

    public static void BeginCommandBuffer(VulkanRenderer renderer, CommandBuffer buffer, ref Framebuffer swapchainImage) {
        API.Vk.ResetCommandBuffer(buffer, CommandBufferResetFlags.None);
        
        { // Begin CommandBuffer
            CommandBufferBeginInfo beginInfo = new() {
                SType = StructureType.CommandBufferBeginInfo,
                Flags = CommandBufferUsageFlags.None,
                PInheritanceInfo = Pointer<CommandBufferInheritanceInfo>.FromNull() // NOTE: Only relevant for secondary buffers.
            };

            _ = Utils.Check(API.Vk.BeginCommandBuffer(buffer, in beginInfo), renderer.Logger, "Failed to begin recording a command buffer", true);
        }
        
        { // Command Begin RenderPass
            ClearValue clearValue = new(new(float32_0:renderer.Settings.ClearValue.R, float32_1:renderer.Settings.ClearValue.G,float32_2:renderer.Settings.ClearValue.B, float32_3: renderer.Settings.ClearValue.A));
            // NOTE: Clear values for load operation clear.

            RenderPassBeginInfo renderPassInfo = new() {
                SType = StructureType.RenderPassBeginInfo,

                RenderPass = renderer.RenderPass.renderPass,
                Framebuffer = swapchainImage,

                RenderArea = new(new(0, 0), renderer.Swapchain.extent), // NOTE: Shader loads, stores area

                ClearValueCount = 1, // Reference the clear value.
                PClearValues = Pointer<ClearValue>.From(ref clearValue)
            };
            
            API.Vk.CmdBeginRenderPass(buffer, in renderPassInfo, SubpassContents.Inline);
            // NOTE: Inline: renderpass setup commands will be embedded in the primary command buffer.
        }
        API.Vk.CmdSetViewport(buffer, 0, 1, in renderer.Swapchain.viewport);
    }

    public static void EndCommandBuffer(VulkanRenderer renderer, CommandBuffer buffer) {
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