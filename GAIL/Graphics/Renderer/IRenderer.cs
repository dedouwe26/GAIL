using System.Diagnostics.CodeAnalysis;
using GAIL.Graphics.Material;
using GAIL.Graphics.Renderer.Layer;

namespace GAIL.Graphics.Renderer;

/// <summary>
/// Represents a class that can render stuff.
/// </summary>
public interface IRenderer<TLayer> : IDisposable, IRendererSettings<TLayer> where TLayer : ILayerSettings {
	public void MoveLayer(int layerIndex, int newIndex);
	public void RemoveLayerAt(int index);
	public bool TryGetLayer(int index, [NotNullWhen(true)] out IBackendLayer? layer);
	public IRasterizationLayer? AddRasterizationLayer(RasterizationLayerSettings settings);
	public IRasterizationLayer? InsertRasterizationLayer(int index, RasterizationLayerSettings settings);

	/// <summary>
	/// Renders the current frame.
	/// </summary>
	public void Render();
	/// <summary>
	/// Resizes the renderer output.
	/// </summary>
	/// <param name="width">The new width.</param>
	/// <param name="height">The new height.</param>
	public void Resize(int width, int height);
	/// <summary>
	/// Creates a shader from the following stages.
	/// </summary>
	/// TODO: Add required stuff
	/// <param name="vertexShader">The per-vertex shader (in SPIR-V compiled).</param>
	/// <param name="fragmentShader">The per-pixel shader (in SPIR-V compiled).</param>
	/// <param name="geometryShader">The geometry shader (in SPIR-V compiled).</param>
	/// <returns>The shader, if it could create a shader.</returns>
	public IShader? CreateShader(FormatInfo[] requiredAttributes, FormatInfo[] requiredUniforms, byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null);
}