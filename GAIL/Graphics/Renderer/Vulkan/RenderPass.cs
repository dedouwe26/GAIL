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
	private readonly Device device;
	public RenderPass(VulkanRenderer renderer) {
		device = renderer.device;
		
		// Stuff about framebuffers.
		AttachmentDescription colorAttachment = new() {
			Format = renderer.Swapchain!.imageFormat,
			Samples = SampleCountFlags.Count1Bit, // NOTE: Can enable MSAA.

			LoadOp = AttachmentLoadOp.Clear, // NOTE: Clears framebuffer to black (beginning of frame).
			StoreOp = AttachmentStoreOp.Store, // NOTE: Stores the rendered frame in the framebuffer (end of frame).

			StencilLoadOp = AttachmentLoadOp.DontCare, // NOTE: Depth buffers (ignored, because it is a color format).
			StencilStoreOp = AttachmentStoreOp.DontCare,

			InitialLayout = ImageLayout.Undefined,
			FinalLayout = ImageLayout.PresentSrcKhr // NOTE: The layout of the image bytes. In this case the layout for the swapchain.
		};
		
		AttachmentDescription[] attachments = [colorAttachment];

		SubpassDependency[] dependencies = new SubpassDependency[1]; // TODO: Temp.
		SubpassDescription[] subpasses = new SubpassDescription[1]; // TODO: Temp.

		//   <<< LAYER SPECIFIC >>>

		for (uint i = 0; i < subpasses.Length; i++) {
			SubpassDependency dependency = new() {
				SrcSubpass = i==0 ? Vk.SubpassExternal : checked(i-1), // NOTE: From.
				DstSubpass = i, // NOTE: To (subpass index).
				
				SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit, // NOTE: On what to wait (swapchain image reading).
				SrcAccessMask = 0, // NOTE: Where that happens.

				DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit, // NOTE: Where to wait.
				DstAccessMask = AccessFlags.ColorAttachmentWriteBit // NOTE: Writing of the color attachment.
			};

			// NOTE: Subpasses are rendering operations that depend on the attachments of previous subpasses.
			// Reference to colorAttachment
			AttachmentReference colorAttachmentRef = new() {
				Attachment = 0, // NOTE: Index of attachment.
				Layout = ImageLayout.ColorAttachmentOptimal // NOTE: Subpass uses it as a color attachment.
			};

			SubpassDescription subpass = new() {
				PipelineBindPoint = renderer.Settings.Layers[i].Pipeline.Type, // NOTE: The type of the pipeline.

				ColorAttachmentCount = 1, // Attachment reference
				PColorAttachments = Pointer<AttachmentReference>.From(ref colorAttachmentRef),
				
				// TODO: Add more attachments.
			};

			dependencies[i] = dependency;
			subpasses[i] = subpass;
		}

		// <<< END LAYER SPECIFIC >>>

		// Creating the render pass.
		RenderPassCreateInfo createInfo = new() {
			SType = StructureType.RenderPassCreateInfo,

			AttachmentCount = Convert.ToUInt32(attachments.LongLength), // NOTE: The attachments list, where the attachment references take from.
			PAttachments = Pointer<AttachmentDescription>.FromArray(ref attachments),

			SubpassCount = Convert.ToUInt32(subpasses.LongLength), // NOTE: Subpass list.
			PSubpasses = Pointer<SubpassDescription>.FromArray(ref subpasses),

			DependencyCount = Convert.ToUInt32(dependencies.LongLength), // NOTE: The dependencies.
			PDependencies = Pointer<SubpassDependency>.FromArray(ref dependencies)
		};

		renderer.Logger.LogDebug("Creating RenderPass.");

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