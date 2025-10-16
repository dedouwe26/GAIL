using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;

namespace GAIL.Storage.Members;

/// <summary>
/// A container can contain more members with keys.
/// </summary>
public sealed class Container : Node, IField {
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

	public void Parse(Parser parser, IFormatter? formatter = null)
	{
		throw new NotImplementedException();
	}

	public void Serialize(Serializer serializer, IFormatter? formatter = null)
	{
		throw new NotImplementedException();
	}
}