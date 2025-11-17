using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer;

public struct LayerDescription {
	public static LayerDescription[] From() {
		return [.. layers.Select((l) => new LayerDescription() { type = l.Pipeline.Type })];
	}
	public PipelineBindPoint type;
}
