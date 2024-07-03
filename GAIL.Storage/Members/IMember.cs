namespace GAIL.Storage.Members;

/// <summary>
/// Represents any member of tree and storage file.
/// </summary>
public interface IMember {
    /// <summary>
    /// The type of this member.
    /// </summary>
    public MemberType Type { get; }
    /// <summary>
    /// The key of this node (cannot contain a dot).
    /// </summary>
    public string Key { get; }
    /// <summary>
    /// The parent of this node, null if it has no parent.
    /// </summary>
    public IParentNode? Parent { get; }

    /// <summary>
    /// Creates the total ID from the root (root.childnode.leafnode).
    /// </summary>
    /// <returns>The ID from the root.</returns>
    public string GetID();

    /// <summary>
    /// Removes the parent.
    /// </summary>
    public void ClearParent();
    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="parent">The new parent.</param>
    public void SetParent(IParentNode parent);
}