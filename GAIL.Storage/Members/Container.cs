namespace GAIL.Storage.Members;

/// <summary>
/// A container can contain more members with keys.
/// </summary>
public sealed class Container : Node, IMember {
    /// <summary>
    /// Creates a new container.
    /// </summary>
    /// <param name="key">The key used by this container node.</param>
    /// <param name="members">The members to add (default: empty).</param>
    public Container(string key, Dictionary<string, IMember>? members = null) : base(key) {
        children = members??[];
    }

    /// <summary>
    /// Creates a new container.
    /// </summary>
    /// <param name="key">The key used by this container node.</param>
    /// <param name="parent">The parent of this container.</param>
    /// <param name="members">The members to add (default: empty).</param>
    public Container(string key, IParentNode parent, Dictionary<string, IMember>? members = null) : base(key) {
        children = members??[];
        SetParent(parent);
    }

    /// <inheritdoc/>
    public override MemberType Type => MemberType.Container;
}