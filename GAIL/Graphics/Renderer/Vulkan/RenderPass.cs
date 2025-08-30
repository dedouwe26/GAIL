using GAIL.Core;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class RenderPass : IDisposable {
	/// <summary>
	/// If this class is already disposed.
	/// </summary>
	public bool IsDisposed { get; private set; }
	public bool AreFramebuffersDisposed { get; private set; } = true;

	public readonly Silk.NET.Vulkan.RenderPass renderPass;
	public Framebuffer[]? Framebuffers { get; private set; }
	private readonly Renderer renderer;
	
	public RenderPass(Renderer renderer) {
		this.renderer = renderer;

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

		SubpassDependency[] dependencies = new SubpassDependency[renderer.layerDescriptions.Length];
		SubpassDescription[] subpasses = new SubpassDescription[renderer.layerDescriptions.Length];

		//   <<< LAYER SPECIFIC >>>

		for (uint i = 0; i < subpasses.Length; i++) {
			SubpassDependency dependency = new() {
				SrcSubpass = i==0 ? Vk.SubpassExternal : checked(i-1), // NOTE: From.
				DstSubpass = i, // NOTE: To (subpass index).
				
				SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit, // NOTE: Where to take.
				SrcAccessMask = 0, // NOTE: What access is allowed.

				DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit, // NOTE: Where to put.
				DstAccessMask = AccessFlags.ColorAttachmentWriteBit // NOTE: What access is allowed.
			};

			// NOTE: Subpasses are rendering operations that depend on the attachments of previous subpasses.
			// Reference to colorAttachment
			AttachmentReference colorAttachmentRef = new() {
				Attachment = 0, // NOTE: Index of attachment, see attachments
				Layout = ImageLayout.ColorAttachmentOptimal // NOTE: Subpass uses it as a color attachment.
			};

			SubpassDescription subpass = new() {
				PipelineBindPoint = renderer.layerDescriptions[i].type, // NOTE: The type of the pipeline.

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

			SubpassCount = Convert.ToUInt32(subpasses.Length), // NOTE: Subpass list.
			PSubpasses = Pointer<SubpassDescription>.FromArray(ref subpasses),

			DependencyCount = Convert.ToUInt32(dependencies.LongLength), // NOTE: The dependencies.
			PDependencies = Pointer<SubpassDependency>.FromArray(ref dependencies)
		};

		renderer.Logger.LogDebug("Creating RenderPass.");

		unsafe {
			_ = Utils.Check(API.Vk.CreateRenderPass(renderer.device.logicalDevice, in createInfo, Allocator.allocatorPtr, out renderPass), renderer.Logger, "Failed to create render pass", true);
		}
	}
	public void CreateFramebuffers() {
		if (!AreFramebuffersDisposed) DisposeFramebuffers();

		Framebuffers = new Framebuffer[renderer.Swapchain.imageViews.Length];

		renderer.Logger.LogDebug("Creating Framebuffers.");

		for (int i = 0; i < renderer.Swapchain.imageViews.Length; i++) {
			ImageView imageView = renderer.Swapchain.imageViews[i];
			FramebufferCreateInfo createInfo = new() {
				SType = StructureType.FramebufferCreateInfo,

				RenderPass = renderPass,
				AttachmentCount = 1,
				PAttachments = Pointer<ImageView>.From(ref imageView),
				Width = renderer.Swapchain.extent.Width,
				Height = renderer.Swapchain.extent.Height,
				Layers = 1
			};

			unsafe {
				_ = Utils.Check(API.Vk.CreateFramebuffer(renderer.device.logicalDevice, Pointer<FramebufferCreateInfo>.From(ref createInfo), Allocator.allocatorPtr, Pointer<Framebuffer>.From(ref Framebuffers[i])), renderer.Logger, "Failed to create framebuffer", true);
			}
		}

		AreFramebuffersDisposed = false;
	}
	public void DisposeFramebuffers() {
		if (AreFramebuffersDisposed) { return; }

		foreach (Framebuffer framebuffer in Framebuffers!) {
			unsafe {
				API.Vk.DestroyFramebuffer(renderer.device.logicalDevice, framebuffer, Allocator.allocatorPtr);
			}
		}

		AreFramebuffersDisposed = true;
	}
	/// <inheritdoc/>
	public void Dispose() {
		if (IsDisposed) { return; }

		DisposeFramebuffers();

		unsafe {
			API.Vk.DestroyRenderPass(renderer.device.logicalDevice, renderPass, Allocator.allocatorPtr);
		}

		IsDisposed = true;
		GC.SuppressFinalize(this);
	}
}