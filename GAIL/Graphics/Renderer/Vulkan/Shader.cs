using System.Collections.ObjectModel;
using GAIL.Core;
using GAIL.Graphics.Material;
using OxDED.Terminal.Logging;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

/// <summary>
/// Represents all the programmable shader stages in a default graphics pipeline.
/// </summary>
public class Shader : IShader {
    public static Shader? CreateShader(VulkanRenderer renderer, ReadOnlyCollection<AttributeType> requiredAttributes, ReadOnlyCollection<AttributeType> requiredUniforms, byte[] vertex, byte[]? fragment = null, byte[]? geometry = null) {
        Shader shader = new(renderer, requiredAttributes, requiredUniforms, 1 + (fragment!=null?1:0) + (geometry!=null?1:0));

        {
            ShaderModule? module;
            if ((module=shader.CreateShaderModule(vertex))==null) {
                return null;
            }
            shader.vertexModule = module.Value;
        }

        shader.IsDisposed = false;

        shader.stages[0] = shader.CreateShaderStage(shader.vertexModule, ShaderStageFlags.VertexBit);

        if (fragment!=null) {
            if ((shader.fragmentModule=shader.CreateShaderModule(fragment))==null) {
                return null;
            }
            shader.stages[1] = shader.CreateShaderStage(shader.fragmentModule.Value, ShaderStageFlags.FragmentBit);
        }

        if (geometry!=null) {
            if ((shader.geometryModule=shader.CreateShaderModule(geometry))==null) {
                return null;
            }
            shader.stages[1+(fragment!=null?1:0)] = shader.CreateShaderStage(shader.geometryModule.Value, ShaderStageFlags.GeometryBit);
        }
        // TODO: Create tesselation (control shader, evaluation shader).

        return shader;
    }
    public PipelineShaderStageCreateInfo[] stages;
    public ShaderModule vertexModule { get; private set; }
    public ShaderModule? fragmentModule { get; private set; }
    public ShaderModule? geometryModule { get; private set; }
    
    /// <summary>
    /// If this class is already disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }
    private readonly ReadOnlyCollection<AttributeType> requiredAttributes;

    private readonly ReadOnlyCollection<AttributeType> requiredUniforms;

    /// <inheritdoc/>
    public ReadOnlyCollection<AttributeType> RequiredAttributes => requiredAttributes;

    /// <inheritdoc/>
    public ReadOnlyCollection<AttributeType> RequiredUniforms => requiredUniforms;

    private readonly Logger Logger;
    private readonly Device Device;
    private Shader(VulkanRenderer renderer, ReadOnlyCollection<AttributeType> requiredAttributes, ReadOnlyCollection<AttributeType> requiredUniforms, int stagesLength) {
        this.requiredAttributes = requiredAttributes;
        this.requiredUniforms = requiredUniforms;
        IsDisposed = true;
        stages = new PipelineShaderStageCreateInfo[stagesLength];
        Logger = renderer.Logger;
        Device = renderer.device;
    }
    public PipelineShaderStageCreateInfo CreateShaderStage(ShaderModule shaderModule, ShaderStageFlags stage) {
        Logger.LogDebug("Creating Shader Stage: "+stage.ToString());

        unsafe {
            PipelineShaderStageCreateInfo createInfo = new() {
                SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = stage,
                Module = shaderModule,
                PName = (byte*)SilkMarshal.StringToPtr("main") // TODO: Needs deallocation. // NOTE: The entry point of the shader.
            };

            return createInfo;
        }
    }
    public ShaderModule? CreateShaderModule(byte[] byteCode) {
        ShaderModuleCreateInfo createInfo = new() {
            SType = StructureType.ShaderModuleCreateInfo,

            CodeSize = (nuint)byteCode.Length
        };

        ShaderModule shaderModule;
        unsafe {
            createInfo.PCode = Pointer<byte>.FromArray(ref byteCode).Cast<uint>();

            if (!Utils.Check(API.Vk.CreateShaderModule(Device.logicalDevice, createInfo, Allocator.allocatorPtr, out shaderModule), Logger, "Failed to create shader module")) {
                return null;
            }
        }
        return shaderModule;
    }

    public VertexInputAttributeDescription[] GetAttributesDescription() {
        VertexInputAttributeDescription[] descriptions = new VertexInputAttributeDescription[requiredAttributes.Count];

        for (uint i = 0; i < requiredAttributes.Count; i++) {
            AttributeType type = requiredAttributes[(int)i];
            descriptions[i] = new VertexInputAttributeDescription() {
                Binding = 0, // TODO: Change this.
                Location = i, // TODO: depending on size of attribute.
                Format = (Format)type,
                Offset = ... // TODO: depending on size of attribute.
            };
        }

        return descriptions;
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (IsDisposed) { return; }

        unsafe {
            API.Vk.DestroyShaderModule(Device.logicalDevice, vertexModule, Allocator.allocatorPtr);
            if (fragmentModule!=null) API.Vk.DestroyShaderModule(Device.logicalDevice, fragmentModule.Value, Allocator.allocatorPtr);
            if (geometryModule!=null) API.Vk.DestroyShaderModule(Device.logicalDevice, geometryModule.Value, Allocator.allocatorPtr);

            foreach (PipelineShaderStageCreateInfo createInfo in stages) {
                SilkMarshal.Free((nint)createInfo.PName);
            }
        }

        IsDisposed = true;

        GC.SuppressFinalize(this);
    }
}