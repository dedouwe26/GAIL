using System.Reflection;
using GAIL.Core;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

public class Pipeline : IDisposable {
    public readonly Silk.NET.Vulkan.Pipeline graphicsPipeline;
    public readonly PipelineLayout layout;
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
            Extent = renderer.Swapchain!.extent,
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

        DynamicState dynamicState = DynamicState.Viewport;

        // Dynamic state info.
        PipelineDynamicStateCreateInfo dynamicStateInfo = new() {
            SType = StructureType.PipelineDynamicStateCreateInfo,

            DynamicStateCount = 1, // NOTE: Using dynamic state for viewport.
            PDynamicStates = Pointer<DynamicState>.From(ref dynamicState)
        };

        // The pipeline layout (used to define uniform values / push constants / descriptor sets).
        PipelineLayoutCreateInfo layoutInfo = new() {
            SType = StructureType.PipelineLayoutCreateInfo,
            SetLayoutCount = 0,
            PSetLayouts = Pointer<DescriptorSetLayout>.FromNull(),
            PushConstantRangeCount = 0,
            PPushConstantRanges = Pointer<PushConstantRange>.FromNull()
        };

        renderer.Logger.LogDebug("Creating Pipeline Layout.");

        unsafe {
            _ = Utils.Check(API.Vk.CreatePipelineLayout(device.logicalDevice, layoutInfo, Allocator.allocatorPtr, out layout), renderer.Logger, "Failed to create the pipeline layout", true);
        }

        

        // Creating the pipeline.
        GraphicsPipelineCreateInfo createInfo = new() {
            SType = StructureType.GraphicsPipelineCreateInfo,

            StageCount = Convert.ToUInt32(shaders.stages!.Length),
            PStages = Pointer<PipelineShaderStageCreateInfo>.FromArray(ref shaders.stages),

            PVertexInputState = Pointer<PipelineVertexInputStateCreateInfo>.From(ref vertexInputInfo),
            PInputAssemblyState = Pointer<PipelineInputAssemblyStateCreateInfo>.From(ref inputAssemblyInfo),
            PViewportState = Pointer<PipelineViewportStateCreateInfo>.From(ref viewportInfo),
            PRasterizationState = Pointer<PipelineRasterizationStateCreateInfo>.From(ref rasterizerInfo),
            PMultisampleState = Pointer<PipelineMultisampleStateCreateInfo>.From(ref multisamplingInfo),
            PDepthStencilState = Pointer<PipelineDepthStencilStateCreateInfo>.FromNull(),
            PColorBlendState = Pointer<PipelineColorBlendStateCreateInfo>.From(ref colorBlendStateInfo),
            PDynamicState = Pointer<PipelineDynamicStateCreateInfo>.From(ref dynamicStateInfo),

            Layout = layout,

            RenderPass = renderer.renderPass!.renderPass,
            Subpass = renderer.renderPass!.graphicsPipelineSubpass, // NOTE: Index of subpass where the graphics pipeline will be used.
            
            BasePipelineHandle = default,
            BasePipelineIndex = -1 // NOTE: Can make pipeline derive from another, to make creating another one less expensive.
        };

        renderer.Logger.LogDebug("Creating Graphics Pipeline.");

        unsafe {
            _ = Utils.Check(API.Vk.CreateGraphicsPipelines(device.logicalDevice, default, 1, in createInfo, Allocator.allocatorPtr, out graphicsPipeline), renderer.Logger, "Failed to create graphics pipeline", true);
        }

        shaders.Dispose();
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) { return; }

        unsafe {
            API.Vk.DestroyPipeline(device.logicalDevice, graphicsPipeline, Allocator.allocatorPtr);
            API.Vk.DestroyPipelineLayout(device.logicalDevice, layout, Allocator.allocatorPtr);
        }
        
        IsDisposed = true;

        GC.SuppressFinalize(this);
    }
}