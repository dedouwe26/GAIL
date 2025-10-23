using System.Collections;
using System.Security.Cryptography.X509Certificates;
using GAIL.Serializing;
using GAIL.Storage.Hierarchy;

namespace GAIL.Storage.Members;

/// <summary>
/// A list can contain more members without keys.
/// </summary>
public class ListField<T> : SerializableField<List<T>>, IField<List<T>>, IList<T> where T : IChildNode, IField {
    /// <summary>
    /// Creates the info for a list field.
    /// </summary>
    /// <param name="typeInfo">The info for type <typeparamref name="T"/>.</param>
    /// <returns>A new list field serializable info.</returns>
    public static ISerializable.Info<ListField<T>> CreateInfo(ISerializable.Info typeInfo) {
        return new((p, f) => {
            ListField<T> list = new("", [], typeInfo);
            list.Parse(p, f);
            return list;
        });
    }
    /// <summary>
    /// Creates a list field.
    /// </summary>
    /// <param name="key">The key of the list field.</param>
    /// <param name="values">The values to initially use.</param>
    /// <param name="info">The info of the values to use.</param>
    public ListField(string key, List<T> values, ISerializable.Info info) : base(key, new ListSerializable<T>(values, info)) { }
    /// <summary>
    /// Creates a list field.
    /// </summary>
    /// <param name="key">The key of the list field.</param>
    /// <param name="values">The values to initially use.</param>
    /// <param name="info">The info of the values to use.</param>
    /// <param name="parent"></param>
    public ListField(string key, List<T> values, ISerializable.Info info, IParentNode parent) : base(key, new ListSerializable<T>(values, info), parent) { }

    /// <inheritdoc/>
	public override MemberType Type => MemberType.List;
    /// <inheritdoc/>
    public T this[int index] { get => Serializable.Value[index]; set => Serializable.Value[index] = value; }
    /// <inheritdoc/>
    public int Count => Serializable.Value.Count;
    /// <inheritdoc/>
    public bool IsReadOnly => ((IList)Serializable.Value).IsReadOnly;
    /// <inheritdoc/>
    public void Add(T item) => Serializable.Value.Add(item);
    /// <inheritdoc/>
    public void Clear() => Serializable.Value.Clear();
    /// <inheritdoc/>
    public bool Contains(T item) => Serializable.Value.Contains(item);
    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => Serializable.Value.CopyTo(array, arrayIndex);
    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Serializable.Value.GetEnumerator();
    /// <inheritdoc/>
    public int IndexOf(T item) => Serializable.Value.IndexOf(item);
    /// <inheritdoc/>
    public void Insert(int index, T item) => Serializable.Value.Insert(index, item);
    /// <inheritdoc/>
    public bool Remove(T item) => Serializable.Value.Remove(item);
    /// <inheritdoc/>
    public void RemoveAt(int index) => Serializable.Value.RemoveAt(index);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}