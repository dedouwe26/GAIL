using System.Collections.Immutable;
using GAIL.Core;
using GAIL.Graphics.Material;
using LambdaKit.Logging;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan.Layer;

/// <summary>
/// Represents all the programmable shader stages in a default graphics pipeline.
/// <para/>
/// This is <b>Vulkan specific</b>, only use this when you are working with Vulkan. Use <see cref="IShader"/> instead.
/// </summary>
public class Shader : IShader {
	public static Shader? Create(Renderer renderer,
		IEnumerable<FormatInfo> requiredAttributes, IEnumerable<FormatInfo> requiredUniforms,
		byte[] vertex, byte[]? fragment = null, byte[]? geometry = null
	) {
		Shader shader = new(renderer, requiredAttributes, requiredUniforms, 1 + (fragment!=null?1:0) + (geometry!=null?1:0));

		{
			ShaderModule? module;
			if ((module=shader.CreateShaderModule(vertex))==null) {
				return null;
			}
			shader.VertexModule = module.Value;
		}

		shader.IsDisposed = false;

		int index = 0;
		shader.stages[index++] = shader.CreateShaderStage(shader.VertexModule, ShaderStageFlags.VertexBit);

		if (fragment!=null) {
			if ((shader.FragmentModule=shader.CreateShaderModule(fragment))==null) {
				return null;
			}
			shader.stages[index++] = shader.CreateShaderStage(shader.FragmentModule.Value, ShaderStageFlags.FragmentBit);
		}

		if (geometry!=null) {
			if ((shader.GeometryModule=shader.CreateShaderModule(geometry))==null) {
				return null;
			}
			shader.stages[index++] = shader.CreateShaderStage(shader.GeometryModule.Value, ShaderStageFlags.GeometryBit);
		}
		// TODO: Create tesselation (control shader, evaluation shader).

		return shader;
	}
	public PipelineShaderStageCreateInfo[] stages;
	public ShaderModule VertexModule { get; private set; }
	public ShaderModule? FragmentModule { get; private set; }
	public ShaderModule? GeometryModule { get; private set; }
	
	/// <summary>
	/// If this class is already disposed.
	/// </summary>
	public bool IsDisposed { get; private set; }
	private readonly ImmutableArray<FormatInfo> requiredAttributes;

	private readonly ImmutableArray<FormatInfo> requiredUniforms;

	/// <inheritdoc/>
	public ImmutableArray<FormatInfo> RequiredAttributes => requiredAttributes;

	/// <inheritdoc/>
	public ImmutableArray<FormatInfo> RequiredUniforms => requiredUniforms;

	private readonly Logger Logger;
	private readonly Device Device;
	private Shader(Renderer renderer, IEnumerable<FormatInfo> requiredAttributes, IEnumerable<FormatInfo> requiredUniforms, int stagesLength) {
		this.requiredAttributes = [.. requiredAttributes];
		this.requiredUniforms = [.. requiredUniforms];
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

			if (!Utils.Check(API.Vk.CreateShaderModule(Device.logicalDevice, in createInfo, Allocator.allocatorPtr, out shaderModule), Logger, "Failed to create shader module")) {
				return null;
			}
		}
		return shaderModule;
	}

	private uint? size = null;
	internal uint GetAttributesSize() {
		if (size == null) {
			size = 0;
			foreach (FormatInfo f in requiredAttributes) {
				size += f.size;
			}
		}
		return size.Value;
	}

	public VertexInputAttributeDescription[] GetAttributesDescription() {
		VertexInputAttributeDescription[] descriptions = new VertexInputAttributeDescription[requiredAttributes.Length];

		uint offset = 0;
		for (uint i = 0; i < requiredAttributes.Length; i++) {
			FormatInfo info = requiredAttributes[(int)i];
			// NOTE: Maybe the attribute size has nothing to do with the attribute type?
			descriptions[i] = new VertexInputAttributeDescription() {
				Binding = 0, // NOTE: See reason in GetBindingDescription().
				Location = i, // TODO: depending on size of attribute ??
				Format = (Format)info.type,
				Offset = offset
			};
			offset+=info.size;
		}
		return descriptions;
	}

	/// <summary>
	/// Describes data loading for the vertex input.
	/// </summary>
	/// <returns></returns>
	public VertexInputBindingDescription GetBindingDescription() {
		VertexInputBindingDescription description = new() {
			Binding = 0, // NOTE: All the vertex data will be packed in one array, so 1 binding.
			Stride = GetAttributesSize(),
			InputRate = VertexInputRate.Vertex
		};
		return description;
	}

	/// <inheritdoc/>
	public void Dispose() {
		if (IsDisposed) { return; }

		unsafe {
			API.Vk.DestroyShaderModule(Device.logicalDevice, VertexModule, Allocator.allocatorPtr);
			if (FragmentModule!=null) API.Vk.DestroyShaderModule(Device.logicalDevice, FragmentModule.Value, Allocator.allocatorPtr);
			if (GeometryModule!=null) API.Vk.DestroyShaderModule(Device.logicalDevice, GeometryModule.Value, Allocator.allocatorPtr);

			foreach (PipelineShaderStageCreateInfo createInfo in stages) {
				SilkMarshal.Free((nint)createInfo.PName);
			}
		}

		IsDisposed = true;

		GC.SuppressFinalize(this);
	}
}