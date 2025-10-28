using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Streams;

namespace GAIL.Storage.Members;

/// <summary>
/// A container can contain more members with keys.
/// </summary>
public sealed class Container : Node, IField {
	internal static readonly IField.Info Info = new((p, hasKey, f) => {
		Container c = new();
		c.Parse(p, hasKey, f);
		return c;
	}, WriteTypeStatic);

	private Container() : this("") { }
	/// <summary>
	/// Creates a new container.
	/// </summary>
	/// <param name="key">The key used by this container node.</param>
	/// <param name="members">The members to add (default: empty).</param>
	public Container(string key, Dictionary<string, IChildNode>? members = null) : base(key) {
		children = members??[];
	}

	/// <summary>
	/// Creates a new container.
	/// </summary>
	/// <param name="key">The key used by this container node.</param>
	/// <param name="parent">The parent of this container.</param>
	/// <param name="members">The members to add (default: empty).</param>
	public Container(string key, IParentNode parent, Dictionary<string, IChildNode>? members = null) : base(key, parent) {
		children = members??[];
	}

	/// <inheritdoc/>
	public void Parse(Parser parser, bool hasKey, IFormatter? formatter = null) {
		if (formatter != null) {
			parser.Decode(p => Parse(p, hasKey), formatter);
		} else {
			if (hasKey) Key = parser.ReadString();
			children = parser.ReadChildren(hasKey).ToDictionary(static x => x.Key, static x => x as IChildNode);
		}
	}
	/// <inheritdoc/>
	public void Serialize(Serializer serializer, bool hasKey, IFormatter? formatter = null) {
		if (formatter != null) {
			serializer.Encode(s => Serialize(s, hasKey), formatter);
		} else {
			if (hasKey) serializer.WriteString(Key);
			serializer.WriteChildren([.. children.Values], hasKey);
		}
	}

	void ISerializable.Parse(Parser parser, IFormatter? formatter) {
		Parse(parser, true, formatter);
	}
	void ISerializable.Serialize(Serializer serializer, IFormatter? formatter) {
		Serialize(serializer, true, formatter);
	}

	private static void WriteTypeStatic(Serializer serializer) {
		serializer.WriteType(MemberType.Container);
	}
	/// <inheritdoc/>
	public void WriteType(Serializer serializer) {
		WriteTypeStatic(serializer);
	}
}