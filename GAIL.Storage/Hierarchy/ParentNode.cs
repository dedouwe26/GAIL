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