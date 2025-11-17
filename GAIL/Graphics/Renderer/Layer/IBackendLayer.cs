using System.Diagnostics.CodeAnalysis;

namespace GAIL.Graphics.Renderer.Layer;

/// <summary>
/// Represents an abstraction for a back-end layer.
/// </summary>
public interface IBackendLayer : IDisposable, ILayerSettings {
    // /// <summary>
    // /// Renders the back-end layer.
    // /// </summary>
    // /// <param name="currentFrame">The current frame in flight.</param>
    // public void Render(uint currentFrame);
    /// <summary>
    /// Whether the renderer should re-record the command buffer.
    /// </summary>
    public bool ShouldRecord { get; }

	public void MoveObject(int layerIndex, int newIndex);
	public void RemoveObjectAt(int index);
	public bool TryGetObject(int index, [NotNullWhen(true)] out Object? layer);
	public void AddObject(Object obj);
	public void InsertObject(int index, Object obj);
}