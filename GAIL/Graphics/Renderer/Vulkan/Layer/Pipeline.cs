using GAIL.Core;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan.Layer;

public class Pipeline : IDisposable {
    public readonly Silk.NET.Vulkan.Pipeline graphicsPipeline;
    public readonly PipelineLayout layout;
    public bool IsDisposed { get; private set; }
    private readonly Device device;

    public readonly PipelineBindPoint Type = PipelineBindPoint.Graphics;

    public Pipeline(VulkanRasterizationLayer layer) {
        device = layer.Renderer.device;

        // Fixed function stages.

        // Creates vertex / index buffers in pipeline
        VertexInputAttributeDescription[] attributeDescriptions = layer.settings.shader.GetAttributesDescription();
        VertexInputBindingDescription bindingDescription = layer.settings.shader.GetBindingDescription();
        PipelineVertexInputStateCreateInfo vertexInputInfo = new (){
            SType = StructureType.PipelineVertexInputStateCreateInfo,
            VertexAttributeDescriptionCount = Convert.ToUInt32(attributeDescriptions.LongLength),
            VertexBindingDescriptionCount = 1,
            PVertexAttributeDescriptions = Pointer<VertexInputAttributeDescription>.FromArray(ref attributeDescriptions),
            PVertexBindingDescriptions = Pointer<VertexInputBindingDescription>.From(ref bindingDescription)
        };

        // Input assembler
        PipelineInputAssemblyStateCreateInfo inputAssemblyInfo = new() {
            SType = StructureType.PipelineInputAssemblyStateCreateInfo,
            Topology = PrimitiveTopology.TriangleList, // NOTE: Can determine how the vertices are interpreted.
            PrimitiveRestartEnable = false
        };
        
        // The scissor of the viewport.
        Rect2D scissor = new() { // TODO: Make configurable??
            Offset = { X = 0, Y = 0 },
            Extent = layer.Renderer.Swapchain!.extent,
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
            PolygonMode = (PolygonMode)layer.settings.FillMode, // NOTE: How fragments are generated (like wireframe: Line).
            LineWidth = 1, // TODO: Make configurable.
            CullMode = (CullModeFlags)layer.settings.CullMode, // NOTE: The way of culling. Most applications use backface culling.
            FrontFace = (FrontFace)layer.settings.FrontFaceMode, // NOTE: How Vulkan knows what the front and back face is.

            DepthBiasEnable = false // NOTE: This can be used to alter the depth value, with the other parameters.
        };

        // Multisampling (MSAA).
        PipelineMultisampleStateCreateInfo multisamplingInfo = new() {
            SType = StructureType.PipelineMultisampleStateCreateInfo,
            SampleShadingEnable = false, // NOTE: Can enable MSAA.
            RasterizationSamples = SampleCountFlags.Count1Bit // NOTE: Can define the count of MSAA.
            // TODO: MSAA: ...
        };

        PipelineDepthStencilStateCreateInfo depthStencilInfo = new() {
            SType = StructureType.PipelineDepthStencilStateCreateInfo,

            DepthTestEnable = false,
            DepthWriteEnable = false,
            DepthCompareOp = CompareOp.Less, // TODO: Make configurable??

            DepthBoundsTestEnable = false, // TODO: Make configurable??
            MinDepthBounds = 0,
            MaxDepthBounds = 1,

            StencilTestEnable = false, // TODO: Make configurable??
            // Front = ...,
            // Back = ...
        };

        // Color blending state.
        PipelineColorBlendAttachmentState colorBlendState = new() {
            ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit,

            BlendEnable = true, // NOTE: Enables blending for multiple fragments on 1 pixel.
            // NOTE: This is alpha blending.
            SrcColorBlendFactor = BlendFactor.SrcAlpha, // TODO: Make configurable??
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

        layer.Logger.LogDebug("Creating Pipeline Layout.");

        unsafe {
            if (!Utils.Check(API.Vk.CreatePipelineLayout(device.logicalDevice, in layoutInfo, Allocator.allocatorPtr, out layout), layer.Logger, "Failed to create the pipeline layout", false)) {
                throw new APIBackendException("Vulkan", "Failed to create the pipeline layout");
            }
        }

        // Creating the pipeline.
        GraphicsPipelineCreateInfo createInfo = new() {
            SType = StructureType.GraphicsPipelineCreateInfo,

            StageCount = Convert.ToUInt32(layer.settings.shader.stages.Length),
            PStages = Pointer<PipelineShaderStageCreateInfo>.FromArray(ref layer.settings.shader.stages),

            PVertexInputState = Pointer<PipelineVertexInputStateCreateInfo>.From(ref vertexInputInfo),
            PInputAssemblyState = Pointer<PipelineInputAssemblyStateCreateInfo>.From(ref inputAssemblyInfo),
            PViewportState = Pointer<PipelineViewportStateCreateInfo>.From(ref viewportInfo),
            PRasterizationState = Pointer<PipelineRasterizationStateCreateInfo>.From(ref rasterizerInfo),
            PMultisampleState = Pointer<PipelineMultisampleStateCreateInfo>.From(ref multisamplingInfo),
            PDepthStencilState = Pointer<PipelineDepthStencilStateCreateInfo>.From(ref depthStencilInfo),
            PColorBlendState = Pointer<PipelineColorBlendStateCreateInfo>.From(ref colorBlendStateInfo),
            PDynamicState = Pointer<PipelineDynamicStateCreateInfo>.From(ref dynamicStateInfo),

            Layout = layout,

            RenderPass = layer.Renderer.RenderPass!.renderPass,
            Subpass = layer.Index, // NOTE: Index of subpass where the graphics pipeline will be used.
            
            BasePipelineHandle = default, // TODO: ???
            BasePipelineIndex = -1 // NOTE: Can make pipeline derive from another, to make creating another one less expensive.
        };

        layer.Logger.LogDebug("Creating Graphics Pipeline.");

        unsafe { // FIXME: AccessViolationException... // NOTE: Subpass index wrong, fix: initialize the renderpass with the pipeline to create both at once.
            if (!Utils.Check(API.Vk.CreateGraphicsPipelines(device.logicalDevice, default, 1, in createInfo, Allocator.allocatorPtr, out graphicsPipeline), layer.Logger, "Failed to create graphics pipeline", false)) {
                throw new APIBackendException("Vulkan", "Failed to create graphics pipeline");
            }
        }
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