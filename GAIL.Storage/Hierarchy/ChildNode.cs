namespace GAIL.Storage.Hierarchy;

/// <summary>
/// The default implementation of <see cref="IChildNode"/>.
/// </summary>
public abstract class ChildNode : IChildNode {
	/// <summary>
	/// Create a node with a key.
	/// </summary>
	/// <param name="key">The key to use.</param>
	public ChildNode(string key) {
		this.key = key;
	}
	/// <summary>
	/// Create a node with a key and a parent.
	/// </summary>
	/// <param name="key">The key to use.</param>
	/// <param name="parent">The parent of this child.</param>
	public ChildNode(string key, IParentNode parent) : this(key) {
		SetParent(parent);
	}

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
		if (Parent == null) return;
		IParentNode previousParent = Parent;
		Parent = null;
		if (previousParent.Children.ContainsKey(Key)) {
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
}