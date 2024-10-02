using System.Reflection;
using GAIL.Core;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class Pipeline : IDisposable {
    public PipelineLayout layout;
    public bool IsDisposed { get; private set; }
    private readonly Device device;

    public Pipeline(VulkanRenderer renderer, Shaders shaders) {
        device = renderer.device!;

        // Fixed function stages.

        // Creates vertex / index buffers in pipeline
        PipelineVertexInputStateCreateInfo vertexInputInfo = new (){
            SType = StructureType.PipelineVertexInputStateCreateInfo,
            VertexAttributeDescriptionCount = 0,
            VertexBindingDescriptionCount = 0,
            // PVertexAttributeDescriptions = null (optional)
            // PVertexBindingDescriptions = null (optional)
        };

        // Input assembler
        PipelineInputAssemblyStateCreateInfo inputAssemblyInfo = new() {
            SType = StructureType.PipelineInputAssemblyStateCreateInfo,
            Topology = PrimitiveTopology.TriangleList, // NOTE: Can determine how the vertices are interpreted.
            PrimitiveRestartEnable = false
        };
        
        // The scissor of the viewport.
        Rect2D scissor = new() {
            Offset = { X = 0, Y = 0 },
            Extent = renderer.swapchain!.extent,
        };

        // NOTE: Using dynamic state so we don't have to specify the viewport.
        // The viewport of the pipeline.
        PipelineViewportStateCreateInfo viewportInfo = new() {
            SType = StructureType.PipelineViewportStateCreateInfo,

            ScissorCount = 1,
            PScissors = Pointer<Rect2D>.From(ref scissor),

            ViewportCount = 1,
            PViewports = Pointer<Viewport>.FromNull()
        };

        // The rasterization stage in the pipeline.
        PipelineRasterizationStateCreateInfo rasterizerInfo = new() {
            SType = StructureType.PipelineRasterizationStateCreateInfo,

            DepthClampEnable = false, // NOTE: Instead of discarding the fragment outside the 
                                      //       near and far planes, it will clamp them.
            RasterizerDiscardEnable = false, // NOTE: Literally disables all rendering (KA-BOOM)!
            PolygonMode = PolygonMode.Fill, // NOTE: How fragments are generated (like wireframe: Line).
            LineWidth = 1,
            CullMode = CullModeFlags.BackBit, // NOTE: The way of culling. Most applications use backface culling.
            FrontFace = FrontFace.Clockwise, // NOTE: How Vulkan knows what the front and back face is.

            DepthBiasEnable = false // NOTE: This can be used to alter the depth value, with the other parameters.
        };

        // Multisampling (MSAA).
        PipelineMultisampleStateCreateInfo multisamplingInfo = new() {
            SType = StructureType.PipelineMultisampleStateCreateInfo,
            SampleShadingEnable = false, // NOTE: Can enable MSAA.
            RasterizationSamples = SampleCountFlags.Count1Bit // NOTE: Can define the count of MSAA.
            // MSAA: ...
        };

        // Color blending state.
        PipelineColorBlendAttachmentState colorBlendState = new() {
            ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit,

            BlendEnable = true, // NOTE: Enables blending for multiple fragments on 1 pixel.
            // NOTE: This is alpha blending.
            SrcColorBlendFactor = BlendFactor.SrcAlpha,
            DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha,
            ColorBlendOp = BlendOp.Add,
            SrcAlphaBlendFactor = BlendFactor.One,
            DstAlphaBlendFactor = BlendFactor.Zero,
            AlphaBlendOp = BlendOp.Add,
        };

        // Color blending info.
        PipelineColorBlendStateCreateInfo colorBlendStateInfo = new() {
            SType = StructureType.PipelineColorBlendStateCreateInfo,

            LogicOpEnable = false, // NOTE: Can enable bitwise combination blending.
            AttachmentCount = 1,
            PAttachments = Pointer<PipelineColorBlendAttachmentState>.From(ref colorBlendState)
        };

        // The pipeline layout (used to define uniform values / push constants / descriptor sets).
        PipelineLayoutCreateInfo layoutInfo = new() {
            SType = StructureType.PipelineLayoutCreateInfo,
            SetLayoutCount = 0,
            PSetLayouts = Pointer<DescriptorSetLayout>.FromNull(),
            PushConstantRangeCount = 0,
            PPushConstantRanges = Pointer<PushConstantRange>.FromNull()
        };

        unsafe {
            _ = Utils.Check(API.Vk.CreatePipelineLayout(device.logicalDevice, layoutInfo, Pointer<AllocationCallbacks>.FromNull(), out layout), renderer.Logger, "Failed to create the pipeline layout", true);
        }

        shaders.Dispose();
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) { return; }

        unsafe {
            API.Vk.DestroyPipelineLayout(device.logicalDevice, layout, Pointer<AllocationCallbacks>.FromNull());
        }
        
        IsDisposed = true;

        GC.SuppressFinalize(this);
    }
}