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
	/// Gets the node of the corresponding id. This is relative to this node.
	/// </summary>
	/// <param name="id">The list of id for what path to take.</param>
	/// <returns>The node if that node exists.</returns>
	public IChildNode? Get(IList<string> id);

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