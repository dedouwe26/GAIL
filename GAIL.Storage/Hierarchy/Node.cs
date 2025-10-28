using GAIL.Serializing;

namespace GAIL.Storage.Hierarchy;

/// <summary>
/// A node is a parent node and a child node.
/// </summary>
public class Node : IParentNode, IChildNode {
	/// <summary>
	/// Creates a node with a key, without a parent.
	/// </summary>
	/// <param name="key">The key of this node.</param>
	public Node(string key)  {
		this.key = key;
	}
	/// <summary>
	/// Create a node with a key, value and a parent.
	/// </summary>
	/// <param name="key">The key</param>
	/// <param name="parent"></param>
	/// <returns></returns>
	public Node(string key, IParentNode parent) : this(key) {
		SetParent(parent);
	}

	/// <inheritdoc/>
	public virtual Dictionary<string, IChildNode> Children => children;
	/// <summary>
	/// The children of this node.
	/// </summary>
	protected Dictionary<string, IChildNode> children = [];

	private string key;
	/// <inheritdoc/>
	public string Key { get => key; set {
		ClearParent();
		IParentNode? previousParent = Parent;
		if (previousParent != null) {
			Parent = null;
			if (previousParent.Children.ContainsKey(Key)) {
				previousParent.RemoveChild(this);
			}
		}
		key = value;
		if (previousParent != null) {
			SetParent(previousParent);
		}
	} }

	/// <inheritdoc/>
	public IParentNode? Parent { get; private set; }

	/// <inheritdoc/>
	public virtual string[] ID { get {
		if (Parent is IChildNode parentChild) {
			return [.. parentChild.ID, Key];
		}
		return [Key];
	} }

	/// <inheritdoc/>
	public virtual void ClearParent() {
		IParentNode? previousParent = Parent;
		Parent = null;
		if (previousParent?.Children.ContainsKey(Key) ?? false) {
			previousParent.RemoveChild(this);
		}
		
	}

	/// <inheritdoc/>
	public virtual void SetParent(IParentNode parent) {
		if (Parent!=null) {
			ClearParent();
		}
		Parent = parent;
		if (!parent.Children.ContainsKey(Key)) {
			parent.AddChild(this);
		}
	}

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
		id.RemoveAt(0);
		if (id.Count <= 0) {
			return child;
		}
		if (child is IParentNode childParent) {
			return childParent.Get(id);
		}
		return null;
	}
}