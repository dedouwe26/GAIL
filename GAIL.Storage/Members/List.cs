using System.Collections;
using GAIL.Storage.Parser;

namespace GAIL.Storage.Members;

/// <summary>
/// A list can contain more members without keys.
/// </summary>
public class List : Member, IMember, IList<IMember> {
    /// <summary>
    /// Creates a new list.
    /// </summary>
    /// <param name="key">The key of the list.</param>
    /// <param name="members">The optional members (default: empty).</param>
    public List(string key, List<IMember>? members = null) : base(key) {
        Members = members??[];
    }
    /// <summary>
    /// Creates a new list.
    /// </summary>
    /// <param name="key">The key of the list.</param>
    /// <param name="parent">The parent of this member.</param>
    /// <param name="members">The optional members (default: empty).</param>
    public List(string key, IParentNode parent, List<IMember>? members = null) : base(key, parent) {
        Members = members??[];
    }

    /// <inheritdoc/>
    public IMember this[int index] { get => Members[index]; set => Members[index] = value; }

    /// <inheritdoc/>
    public override MemberType Type => MemberType.List;
    /// <summary>
    /// The members of this list member.
    /// </summary>
    public List<IMember> Members { get; set; }

    /// <inheritdoc/>
    public int Count => Members.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(IMember item) {
        Members.Add(item);
    }

    /// <inheritdoc/>
    public void Clear() {
        Members.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(IMember item) {
        return Members.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(IMember[] array, int arrayIndex) {
        Members.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<IMember> GetEnumerator() {
        return Members.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(IMember item) {
        return Members.IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, IMember item) {
        Members.Contains(item);
    }

    /// <inheritdoc/>
    public bool Remove(IMember item) {
        return Members.Remove(item);
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)  {
        Members.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}