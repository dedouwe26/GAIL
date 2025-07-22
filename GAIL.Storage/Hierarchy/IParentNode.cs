namespace GAIL.Storage.Hierarchy;

/// <summary>
/// Represents a node that has children.
/// </summary>
public interface IParentNode {
    /// <summary>
    /// The children of this node.
    /// </summary>
    public Dictionary<string, IChildNode> Children { get; }

    /// <summary>
    /// Gets the node of the corresponding ID. This is relative to this node.
    /// </summary>
    /// <param name="ID">The ID of the node to return (can contain dots).</param>
    /// <returns>The node if that node exists.</returns>
    [Obsolete("Use Get(List<string>) instead.")]
    public IChildNode? Get(string ID);
    /// <summary>
    /// Gets the node of the corresponding keys. This is relative to this node.
    /// </summary>
    /// <param name="keys">The list of keys for what path to take.</param>
    /// <returns>The node if that node exists.</returns>
    public IChildNode? Get(List<string> keys);

    /// <summary>
    /// Gets the node of the corresponding ID. This is relative to this node.
    /// </summary>
    /// <typeparam name="T">The type of the child node.</typeparam>
    /// <param name="ID">The ID of the node to return (can contain dots).</param>
    /// <returns>The node if that node exists.</returns>
    [Obsolete("Use Get<T>(List<string>) instead.")]
    public T? Get<T>(string ID) where T : IChildNode;

    /// <summary>
    /// Gets the node of the corresponding ID. This is relative to this node.
    /// </summary>
    /// <typeparam name="T">The type of the child node.</typeparam>
    /// <param name="keys">The list of keys for what path to take.</param>
    /// <returns>The node if that node exists.</returns>
    public T? Get<T>(List<string> keys) where T : IChildNode;

    /// <summary>
    /// Adds a child to this node.
    /// </summary>
    /// <param name="member">The node to add as a child.</param>
    public void AddChild(IChildNode member);
    /// <summary>
    /// Removes a child from this node.
    /// </summary>
    /// <param name="child">The child node to remove.</param>
    /// <returns>True it was successful. Else if it didn't find the node in the list, it is false.</returns>
    public bool RemoveChild(IChildNode child);
}