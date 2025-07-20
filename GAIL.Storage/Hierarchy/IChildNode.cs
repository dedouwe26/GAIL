namespace GAIL.Storage.Hierarchy;

/// <summary>
/// Represents a child of a parent node.
/// </summary>
public interface IChildNode {
    /// <summary>
    /// The key of this node.
    /// </summary>
    public string Key { get; }
    /// <summary>
    /// The parent of this node, null if it has no parent.
    /// </summary>
    public IParentNode? Parent { get; }

    /// <summary>
    /// Gets the total ID from the root (root.childnode.leafnode).
    /// </summary>
    public string ID { get; }

    /// <summary>
    /// Clears the parent of this node.
    /// </summary>
    public void ClearParent();
    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="parent">The new parent.</param>
    public void SetParent(IParentNode parent);
}