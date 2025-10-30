using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan.Layer;

// NOTE: Struct because of size.
public struct LayerDescription {
	public static LayerDescription[] From(IVulkanLayer[] layers) {
		return [.. layers.Select((l) => new LayerDescription() { type = l.Pipeline.Type })];
	}
	public PipelineBindPoint type;
}
