namespace GAIL.Storage;

/// <summary>
/// Creates a new container, a container can contain more containers or fields.
/// </summary>
public sealed class Container : Node {
    /// <summary>
    /// Creates a new container.
    /// </summary>
    /// <param name="key">The key used by this container node.</param>
    public Container(string key) : base(key) { }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="node"/> is not a container or a field.</exception>
    public override void AddChild(IChildNode node) {
        if (node is not Container || node is not Field) {
            throw new ArgumentException("Node is not a container or a field.", nameof(node));
        }
        base.AddChild(node);
    }
    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="node"/> is not a container or a field.</exception>
    public override bool RemoveChild(IChildNode node) {
        if (node is not Container || node is not Field) {
            throw new ArgumentException("Node is not a container or a field.", nameof(node));
        }
        return base.RemoveChild(node);
    }
    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="parent"/> is not a container.</exception>
    public override void SetParent(IParentNode parent) {
        if (parent is not Container || parent is not Storage) {
            throw new ArgumentException("Parent is not a container or a storage node.", nameof(parent));
        }
        base.SetParent(parent);
    }
}