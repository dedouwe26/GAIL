namespace GAIL.Storage.Hierarchy;

/// <summary>
/// The default implementation of <see cref="IChildNode"/>.
/// </summary>
public abstract class ChildNode : IChildNode {
    /// <summary>
    /// Create a node with a key, value and a parent.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="parent">The parent of this child.</param>
    public ChildNode(string key, IParentNode parent) {
        Key = key;
        SetParent(parent);
    }

    /// <inheritdoc/>
    public string Key { get; private set; }

    /// <inheritdoc/>
    public IParentNode? Parent { get; private set; }

    /// <inheritdoc/>
    public virtual string ID { get {
        if (Parent == null) {
            return Key;
        }
        if (Parent is IChildNode parentChild) {
            string parentID = parentChild.ID;
            return parentID+'.'+Key;
        }
        return Key;
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
    /// <exception cref="ArgumentException"><paramref name="parent"/> is not a valid member.</exception>
    public virtual void SetParent(IParentNode parent) {
        // if (parent is IChildNode member) {
        //     if (!StorageRegister.IsMemberRegistered(member)) {
        //         throw new ArgumentException("Parent is not a valid member", nameof(parent));
        //     }
        // } else if (parent is not Storage) {
        //     throw new ArgumentException("Parent is not a valid member", nameof(parent));
        // } TODO: ???
        if (Parent!=null) {
            ClearParent();
        }
        Parent = parent;
        if (!parent.Children.ContainsKey(Key)) {
            parent.AddChild(this);
        }
    }
}