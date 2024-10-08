using System.Reflection;
using GAIL.Core;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class RenderPass : IDisposable {
    /// <summary>
    /// If this class is already disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }
    public readonly Silk.NET.Vulkan.RenderPass renderPass;
    public readonly uint graphicsPipelineSubpass;
    private readonly Device device;
    public RenderPass(VulkanRenderer renderer) {
        device = renderer.device;
        
        // Stuff about framebuffers.
        AttachmentDescription colorAttachment = new() {
            Format = renderer.swapchain!.imageFormat,
            Samples = SampleCountFlags.Count1Bit, // NOTE: Can enable MSAA.

            LoadOp = AttachmentLoadOp.Clear, // NOTE: Clears framebuffer to black.
            StoreOp = AttachmentStoreOp.Store, // NOTE: Stores the rendered frame in the framebuffer.

            StencilLoadOp = AttachmentLoadOp.DontCare, // NOTE: Depth buffers.
            StencilStoreOp = AttachmentStoreOp.DontCare,

            InitialLayout = ImageLayout.Undefined,
            FinalLayout = ImageLayout.PresentSrcKhr // NOTE: The layout of the image bytes. In this case the layout for the swapchain.
        };

        // NOTE: Subpasses are rendering operations that depend on the attachments of previous subpasses.
        // Reference to colorAttachment
        AttachmentReference colorAttachmentRef = new() {
            Attachment = 0, // NOTE: Index of attachment.
            Layout = ImageLayout.ColorAttachmentOptimal // NOTE: Subpass uses it as a color attachment.
        };

        SubpassDescription subpass = new() {
            PipelineBindPoint = PipelineBindPoint.Graphics, // NOTE: This is a graphics subpass.

            ColorAttachmentCount = 1, // Attachment reference
            PColorAttachments = Pointer<AttachmentReference>.From(ref colorAttachmentRef),
        };
        graphicsPipelineSubpass = 0;

        SubpassDependency dependency = new() {
            SrcSubpass = Vk.SubpassExternal, // NOTE: From.
            DstSubpass = 0, // NOTE: To (subpass index).
            
            SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit, // NOTE: On what to wait (swapchain image reading).
            SrcAccessMask = 0, // NOTE: Where that happens.

            DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit, // NOTE: Where to wait.
            DstAccessMask = AccessFlags.ColorAttachmentWriteBit // NOTE: Writing of the color attachment.
        };

        // Creating the render pass.
        RenderPassCreateInfo createInfo = new() {
            SType = StructureType.RenderPassCreateInfo,

            AttachmentCount = 1, // NOTE: The attachments list, where the attachment references take from.
            PAttachments = Pointer<AttachmentDescription>.From(ref colorAttachment),

            SubpassCount = 1, // NOTE: Subpass list.
            PSubpasses = Pointer<SubpassDescription>.From(ref subpass),

            DependencyCount = 1, // NOTE: The dependencies.
            PDependencies = Pointer<SubpassDependency>.From(ref dependency)
        };
        unsafe {
            _ = Utils.Check(API.Vk.CreateRenderPass(device.logicalDevice, in createInfo, Allocator.allocatorPtr, out renderPass), renderer.Logger, "Failed to create render pass", true);
        }
    }
    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) { return; }

        unsafe {
            API.Vk.DestroyRenderPass(device.logicalDevice, renderPass, Allocator.allocatorPtr);
        }

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}