using GAIL.Serializing;

namespace GAIL.Storage.Hierarchy;

/// <summary>
/// The default implementation of <see cref="IParentNode"/>.
/// </summary>
public class ParentNode : IParentNode {
	/// <inheritdoc/>
	public virtual Dictionary<string, IChildNode> Children => children;
	/// <summary>
	/// The children of this node.
	/// </summary>
	protected Dictionary<string, IChildNode> children = [];

	/// <inheritdoc/>
	public virtual void AddChild(IChildNode member) {
		if (children.ContainsKey(member.Key)) { return; }
		
		children.Add(member.Key, member);

		if (member.Parent != this) {
			member.SetParent(this);
		}
	}

	/// <inheritdoc/>
	public virtual bool RemoveChild(IChildNode child) {
		if (!children.ContainsKey(child.Key)) { return false; }
		children.Remove(child.Key);
		if (child.Parent == this) {
			child.ClearParent();
		}
		return true;
	}

	/// <inheritdoc/>
	public virtual IChildNode? Get(IList<string> id) {
		if (children.Count < 1) {
			return null;
		}
		IChildNode? child = children!.GetValueOrDefault(id[0]);
		if (child == null) {
			return null;
		}
		if (id.Count == 1) {
			return child;
		}
		id.RemoveAt(0);
		if (child is IParentNode childParent) {
			return childParent.Get(id);
		}
		return null;
	}
}