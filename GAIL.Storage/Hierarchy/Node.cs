namespace GAIL.Storage.Hierarchy;

/// <summary>
/// A node is a parent node and a child node.
/// </summary>
public class Node : IParentNode, IChildNode {
    /// <summary>
    /// Creates a node with a key, without a parent.
    /// </summary>
    /// <param name="key">The key of this node.</param>
    /// <exception cref="InvalidOperationException">Key cannot contain a dot.</exception>
    public Node(string key)  {
        if (key.Contains('.')) {
            throw new InvalidOperationException("Invalid key, cannot contain dots");
        }
        Key = key;
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

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="member"/> is not registered.</exception>
    public virtual void AddChild(IChildNode member) {
        // if (!StorageRegister.IsMemberRegistered(member)) {
        //     throw new ArgumentException("Member is not registered", nameof(member));
        // } TODO: ???
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
    [Obsolete("Use Get<T>(List<string>) instead.")]
    public virtual T? Get<T>(string ID) where T : IChildNode {
        return (T?)Get(ID);
    }
    /// <inheritdoc/>
    public virtual T? Get<T>(List<string> keys) where T : IChildNode {
        return (T?)Get(keys);
    }

    /// <inheritdoc/>
    [Obsolete("Use Get(List<string>) instead.")]
    public virtual IChildNode? Get(string ID) {
        return Get([.. ID.Split('.')]);
    }

    /// <inheritdoc/>
    public virtual IChildNode? Get(List<string> keys) {
        if (children.Count < 1) {
            return null;
        }
        IChildNode? child = children!.GetValueOrDefault(keys[0]);
        if (child == null) {
            return null;
        }
        keys.RemoveAt(0);
        if (keys.Count <= 0) {
            return child;
        }
        if (child is IParentNode childParent) {
            return childParent.Get(keys);
        }
        return null;
    }
}