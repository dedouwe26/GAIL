using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Members;

namespace GAIL.Storage.Streams;

/// <summary>
/// Adds methods to read storage specific data.
/// </summary>
public static class ParserExtensions {
	/// <summary>
	/// Reads the type from the parser.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <returns>The read member type.</returns>
	public static MemberType ReadType(this Parser parser) {
		return (MemberType)parser.ReadByte();
	}
	/// <summary>
	/// Reads any valid member from the stream.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <param name="hasKey">True if there is a key to read.</param>
	/// <returns>The new parsed member, null if it is an end.</returns>
	public static IChildNode? ReadMember(this Parser parser, bool hasKey = true) {
		MemberType type = ReadType(parser);
		string key = "";
        if (hasKey) {
            key = parser.ReadString();
        }

		return parser.ReadSerializable(info) as IChildNode;
	}
	/// <summary>
	/// Reads multiple members.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <param name="hasKey">If it should read keys.</param>
	/// <returns>A list of parsed members.</returns>
	public static List<IChildNode> ReadChildren(this Parser parser, bool hasKey = true) {
        IChildNode? member;
        try {
            member = ReadMember(parser, hasKey);
        } catch (EndOfStreamException) {
            return [];
        }
        if (member==null) {
            return [];
        } else {
            return [member, .. ReadChildren(parser, hasKey)];
        }
    }
}