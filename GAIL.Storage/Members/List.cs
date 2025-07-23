using System.Collections;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Parser;

namespace GAIL.Storage.Members;

/// <summary>
/// A list can contain more members without keys.
/// </summary>
public class List : IField<IField>, IList<IChildNode> {
    /// <summary>
    /// Creates a new list.
    /// </summary>
    /// <param name="key">The key of the list.</param>
    /// <param name="members">The optional members (default: empty).</param>
    public List(string key, List<IChildNode>? members = null) : base(key) {
        Members = members??[];
    }
    /// <summary>
    /// Creates a new list.
    /// </summary>
    /// <param name="key">The key of the list.</param>
    /// <param name="parent">The parent of this member.</param>
    /// <param name="members">The optional members (default: empty).</param>
    public List(string key, IParentNode parent, List<IChildNode>? members = null) : base(key, parent) {
        Members = members??[];
    }

    /// <inheritdoc/>
    public IChildNode this[int index] { get => Members[index]; set => Members[index] = value; }

    /// <summary>
    /// The members of this list member.
    /// </summary>
    public List<IChildNode> Members { get; set; }

    /// <inheritdoc/>
    public int Count => Members.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    public string Key => throw new NotImplementedException();

    public IParentNode? Parent => throw new NotImplementedException();

    public string ID => throw new NotImplementedException();

    public IField Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public uint? FixedSize => null;

    /// <inheritdoc/>
    public void Add(IChildNode item) {
        Members.Add(item);
    }

    /// <inheritdoc/>
    public void Clear() {
        Members.Clear();
    }

    public void ClearParent()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Contains(IChildNode item) {
        return Members.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(IChildNode[] array, int arrayIndex) {
        Members.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<IChildNode> GetEnumerator() {
        return Members.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(IChildNode item) {
        return Members.IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, IChildNode item) {
        Members.Contains(item);
    }

    public void Parse(byte[] data)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Remove(IChildNode item) {
        return Members.Remove(item);
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)  {
        Members.RemoveAt(index);
    }

    public byte[] Serialize()
    {
        throw new NotImplementedException();
    }

    public void SetParent(IParentNode parent)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}