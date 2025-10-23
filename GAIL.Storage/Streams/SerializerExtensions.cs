using GAIL.Serializing;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Members;

namespace GAIL.Storage.Streams;

/// <summary>
/// Adds methods to write storage specific data.
/// </summary>
public static class SerializerExtensions {
	/// <summary>
	/// Reads the type from the parser.
	/// </summary>
	/// <param name="serializer">The parser to write to.</param>
	/// <param name="type">The type to write.</param>
	public static void WriteType(this Serializer serializer, MemberType type) {
		serializer.WriteByte((byte)type);
	}
    /// <summary>
	/// Writes a member to the serializer.
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	/// <param name="member">The member to serialize.</param>
	/// <param name="hasKey">Whether it should write the key.</param>
    public static void WriteMember(this Serializer serializer, IField member, bool hasKey = true) {
		WriteType(serializer, member.Type);
		member.Serialize(serializer, hasKey, null);
	}
    
    /// <summary>
	/// Writes a members to the serializer.
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	/// <param name="children">The children to serialize.</param>
	/// <param name="hasKey">Whether it should write the keys of the children.</param>
    public static void WriteChildren(this Serializer serializer, IChildNode[] children, bool hasKey = true) {
        foreach (IChildNode child in children) {
            if (child is IField member) {
                WriteMember(serializer, member, hasKey);
            }
        }
    }
}